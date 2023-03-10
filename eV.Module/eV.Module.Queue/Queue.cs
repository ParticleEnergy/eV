// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Reflection;
using eV.Module.EasyLog;
using eV.Module.Queue.Attributes;
using eV.Module.Queue.Interface;
using StackExchange.Redis;

namespace eV.Module.Queue;

public class Queue
{
    private bool _isStart;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly Dictionary<Type, ConsumerIdentifier> _consumerIdentifiers = new();
    private readonly Dictionary<Type, IQueueHandler> _handlers = new();

    private readonly ConnectionMultiplexer _redis;

    private readonly string _project;
    private readonly string _nodeId;

    public Queue(string project, string node, string queueAssemblyString, ConnectionMultiplexer redis)
    {
        _cancellationTokenSource = new CancellationTokenSource();

        _project = project;
        _nodeId = node;
        _redis = redis;

        Register(queueAssemblyString);
    }

    private void Register(string assemblyString)
    {
        if (_isStart)
            return;

        Type[] allTypes = Assembly.Load(assemblyString).GetExportedTypes();

        foreach (Type type in allTypes)
        {
            object[] jobAttributes = type.GetCustomAttributes(typeof(QueueConsumerAttribute), true);

            if (jobAttributes.Length <= 0)
                continue;

            if (Activator.CreateInstance(type) is not IQueueHandler handler)
                continue;

            Type[]? contentTypes = type.BaseType?.GenericTypeArguments;
            if (contentTypes is not { Length: > 0 })
                continue;

            _consumerIdentifiers[contentTypes[0]] = new ConsumerIdentifier(
                $"eV:Queue:{_project}:Stream:{contentTypes[0].Name}",
                $"eV:Queue:{_project}:Group:{contentTypes[0].Name}",
                $"eV:Queue:{_project}:Node:{_nodeId}:Consumer:{contentTypes[0].Name}"
            );

            _handlers[contentTypes[0]] = handler;
            Logger.Info($"Queue Consumer [{type.FullName}] registration succeeded");
        }

        MessageProcessor.InitMessageProcessor(_redis, _consumerIdentifiers);
    }

    private void InitStream(ConsumerIdentifier consumerIdentifier)
    {
        try
        {
            if (_redis.GetDatabase().KeyExists(consumerIdentifier.Stream))
            {
                StreamGroupInfo[] groupInfos = _redis.GetDatabase().StreamGroupInfo(consumerIdentifier.Stream);

                bool groupExists = groupInfos.Any(streamGroupInfo => streamGroupInfo.Name == consumerIdentifier.Group);

                if (groupExists) return;
            }

            _redis.GetDatabase().StreamCreateConsumerGroup(
                consumerIdentifier.Stream,
                consumerIdentifier.Group,
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
        _redis.GetDatabase().StreamDeleteConsumer(
            consumerIdentifier.Stream,
            consumerIdentifier.Group,
            consumerIdentifier.Consumer
        );
    }

    public void Start()
    {
        if (_isStart)
            return;
        if (!_redis.IsConnected)
            return;

        foreach ((Type contentType, IQueueHandler handler) in _handlers)
        {
            if (!_consumerIdentifiers.ContainsKey(contentType))
                continue;

            _consumerIdentifiers.TryGetValue(contentType, out ConsumerIdentifier? consumerIdentifier);
            if (consumerIdentifier == null)
                continue;

            InitStream(consumerIdentifier);

            Task.Run(() => { handler.RunConsume(_cancellationTokenSource.Token); }, _cancellationTokenSource.Token);

            Logger.Info($"Queue [{contentType.FullName}] start consume");
        }

        _isStart = true;
        Logger.Info("Queue start");
    }

    public void Stop()
    {
        if (!_isStart)
            return;

        _cancellationTokenSource.Cancel();

        foreach (var contentType in _handlers.Keys.Where(contentType => _consumerIdentifiers.ContainsKey(contentType)))
        {
            _consumerIdentifiers.TryGetValue(contentType, out ConsumerIdentifier? consumerIdentifier);
            if (consumerIdentifier == null)
                continue;

            ReleaseStream(consumerIdentifier);
        }

        Logger.Info("Queue stop");
    }
}
