// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using System.Reflection;
using eV.Module.Cluster.Attributes;
using eV.Module.Cluster.Interface;
using eV.Module.EasyLog;
using StackExchange.Redis;

namespace eV.Module.Cluster;

public class Cluster
{
    private bool _isStart;

    private readonly string _clusterId;
    private readonly string _nodeId;

    private readonly Dictionary<Type, ChannelIdentifier>  _channelIdentifiers = new();
    private readonly Dictionary<Type, IInternalHandler> _handlers = new();

    private readonly SessionRegistrationAuthority _sessionRegistrationAuthority;
    private readonly ConnectionMultiplexer _redis;

    public Cluster(
        string clusterId,
        string nodeId,
        string clusterAssemblyString,
        ConnectionMultiplexer redis
    )
    {
        _clusterId = clusterId;
        _nodeId = nodeId;
        _redis = redis;

        _sessionRegistrationAuthority = new SessionRegistrationAuthority(_clusterId, _nodeId, _redis);

        Register(clusterAssemblyString);
    }

    private void Register(string assemblyString)
    {
        if (_isStart)
            return;

        Type[] allTypes = Assembly.Load(assemblyString).GetExportedTypes();

        foreach (Type type in allTypes)
        {
            object[] jobAttributes = type.GetCustomAttributes(typeof(ReceiveInternalMessageHandlerAttribute), true);

            if (jobAttributes.Length <= 0)
                continue;

            if (Activator.CreateInstance(type) is not IInternalHandler handler)
                continue;

            Type[]? contentTypes = type.BaseType?.GenericTypeArguments;
            if (contentTypes is not { Length: > 0 })
                continue;

            _channelIdentifiers[contentTypes[0]] = new ChannelIdentifier(_clusterId, contentTypes[0].Name, handler.IsMultipleSubscribers);
            _handlers[contentTypes[0]] = handler;

            Logger.Info($"InternalHandler [{type.FullName}] registration succeeded");
        }
    }

    public void AddCustomHandler(Type handlerType, Type contentType)
    {
        if (Activator.CreateInstance(handlerType) is not IInternalHandler handler)
            return;

        _channelIdentifiers[contentType] = new ChannelIdentifier(_clusterId, contentType.Name, handler.IsMultipleSubscribers);
        _handlers[contentType] = handler;
    }

    public void Start()
    {
        if (_isStart)
            return;
        if (!_redis.IsConnected)
            return;

        _sessionRegistrationAuthority.Start();

        CommunicationManager.InitCommunicationManager
        (
            _nodeId,
            _redis,
            _sessionRegistrationAuthority,
            _channelIdentifiers
        );

        // Internal
        foreach ((Type contentType, IInternalHandler handler) in _handlers)
        {
            if (!_channelIdentifiers.ContainsKey(contentType))
                continue;

            _channelIdentifiers.TryGetValue(contentType, out ChannelIdentifier? channelIdentifier);
            if (channelIdentifier == null)
                continue;

            handler.Run();
        }

        _isStart = true;

        Logger.Info("Cluster start");
    }

    public void Stop()
    {
        if (!_isStart)
            return;

        _sessionRegistrationAuthority.Stop();
        Logger.Info("Cluster stop");
    }
}
