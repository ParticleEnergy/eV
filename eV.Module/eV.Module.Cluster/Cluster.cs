// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using System.Reflection;
using eV.Module.Cluster.Attributes;
using eV.Module.Cluster.Communication;
using eV.Module.Cluster.Interface;
using eV.Module.EasyLog;
using StackExchange.Redis;

namespace eV.Module.Cluster;

public class Cluster
{
    private bool _isStart;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly string _clusterId;
    private readonly string _nodeId;

    private readonly Dictionary<Type, ConsumerIdentifier> _consumerIdentifiers = new();
    private readonly Dictionary<Type, IInternalHandler> _handlers = new();

    private readonly SessionRegistrationAuthority _sessionRegistrationAuthority;
    private readonly ConnectionMultiplexer _redis;

    private readonly Send _send;
    private readonly SendBroadcast _sendBroadcast;

    public Cluster(
        string clusterId,
        string nodeId,
        string clusterAssemblyString,
        ConnectionMultiplexer redis,
        CommunicationSetting communicationSetting
    )
    {
        _cancellationTokenSource = new CancellationTokenSource();

        _clusterId = clusterId;
        _nodeId = nodeId;
        _redis = redis;

        _sessionRegistrationAuthority = new SessionRegistrationAuthority(_clusterId, _nodeId, _redis);

        _send = new Send(_clusterId, communicationSetting.SendAction, communicationSetting.SendBatchProcessingQuantity);
        _sendBroadcast = new SendBroadcast(_clusterId, communicationSetting.SendBroadcastAction, communicationSetting.SendBroadcastBatchProcessingQuantity);

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

            _consumerIdentifiers[contentTypes[0]] = new ConsumerIdentifier(_clusterId, contentTypes[0].Name);

            _handlers[type] = handler;
            Logger.Info($"InternalHandler [{type.FullName}] registration succeeded");
        }

        CommunicationManager.InitCommunicationManager
        (
            _nodeId,
            _redis,
            _sessionRegistrationAuthority,
            _consumerIdentifiers,
            _send.ConsumerIdentifiers,
            _sendBroadcast.ConsumerIdentifiers
        );
    }

    private void InitStream(ConsumerIdentifier consumerIdentifier)
    {
        try
        {
            if (_redis.GetDatabase().KeyExists(consumerIdentifier.GetStream(_nodeId)))
            {
                StreamGroupInfo[] groupInfos = _redis.GetDatabase().StreamGroupInfo(consumerIdentifier.GetStream(_nodeId));

                bool groupExists = groupInfos.Any(streamGroupInfo => streamGroupInfo.Name == consumerIdentifier.GetGroup(_nodeId));

                if (groupExists) return;
            }

            _redis.GetDatabase().StreamCreateConsumerGroup(
                consumerIdentifier.GetStream(_nodeId),
                consumerIdentifier.GetGroup(_nodeId),
                StreamPosition.NewMessages
            );
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }

    private void ReleaseStream(ConsumerIdentifier consumerIdentifier)
    {
        try
        {
            var batch = _redis.GetDatabase().CreateBatch();

            // 删除消费者
            batch.StreamDeleteConsumerAsync(
                consumerIdentifier.GetStream(_nodeId),
                consumerIdentifier.GetGroup(_nodeId),
                consumerIdentifier.GetConsumer(_nodeId)
            );

            // 删除组
            batch.StreamDeleteConsumerGroupAsync(
                consumerIdentifier.GetStream(_nodeId),
                consumerIdentifier.GetGroup(_nodeId)
            );

            // 删除流
            batch.KeyDeleteAsync(consumerIdentifier.GetStream(_nodeId));

            batch.Execute();
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }

    public void Start()
    {
        if (_isStart)
            return;
        if (!_redis.IsConnected)
            return;

        _sessionRegistrationAuthority.Start();

        // Send
        foreach (ConsumerIdentifier consumerIdentifier in _send.ConsumerIdentifiers.Values)
        {
            InitStream(consumerIdentifier);
        }

        _send.Run(_cancellationTokenSource.Token);
        Logger.Info("Cluster Send start consume");

        // SendBroadcast
        foreach (ConsumerIdentifier consumerIdentifier in _sendBroadcast.ConsumerIdentifiers.Values)
        {
            InitStream(consumerIdentifier);
        }

        _sendBroadcast.Run(_cancellationTokenSource.Token);
        Logger.Info("Cluster SendBroadcast start consume");

        // Internal
        foreach ((Type contentType, IInternalHandler handler) in _handlers)
        {
            if (!_consumerIdentifiers.ContainsKey(contentType))
                continue;

            _consumerIdentifiers.TryGetValue(contentType, out ConsumerIdentifier? consumerIdentifier);
            if (consumerIdentifier == null)
                continue;

            InitStream(consumerIdentifier);

            Task.Run(() => { handler.Run(_cancellationTokenSource.Token); }, _cancellationTokenSource.Token);

            Logger.Info($"ReceiveInternalHandler [{contentType.FullName}] start consume");
        }

        _isStart = true;

        Logger.Info("Cluster start");
    }

    public void Stop()
    {
        if (!_isStart)
            return;

        _cancellationTokenSource.Cancel();

        _sessionRegistrationAuthority.Stop();

        // Send
        foreach (ConsumerIdentifier consumerIdentifier in _send.ConsumerIdentifiers.Values)
        {
            ReleaseStream(consumerIdentifier);
        }

        // SendBroadcast
        foreach (ConsumerIdentifier consumerIdentifier in _sendBroadcast.ConsumerIdentifiers.Values)
        {
            ReleaseStream(consumerIdentifier);
        }

        foreach (Type contentType in _handlers.Keys)
        {
            if (!_consumerIdentifiers.ContainsKey(contentType))
                continue;

            _consumerIdentifiers.TryGetValue(contentType, out ConsumerIdentifier? consumerIdentifier);
            if (consumerIdentifier == null)
                continue;

            ReleaseStream(consumerIdentifier);
        }

        Logger.Info("Cluster stop");
    }
}
