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
        MessageProcessor.InitMessageProcessor(_redis, _consumerIdentifiers);
    }

    private void Register(string assemblyString)
    {
        if (_isStart)
            return;

        Type[] allTypes = Assembly.Load(assemblyString).GetExportedTypes();

        foreach (Type type in allTypes)
        {
            object[] jobAttributes = type.GetCustomAttributes(typeof(QueueAttribute), true);

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

            _handlers[type] = handler;
            Logger.Info($"Queue [{type.FullName}] registration succeeded");
        }
    }

    private async Task InitStream(ConsumerIdentifier consumerIdentifier)
    {
        StreamGroupInfo[] groupInfos = await _redis.GetDatabase().StreamGroupInfoAsync(consumerIdentifier.Stream);
        bool groupExists = groupInfos.Any(streamGroupInfo => streamGroupInfo.Name == consumerIdentifier.Group);

        if (groupExists) return;

        bool createStream = !await _redis.GetDatabase().KeyExistsAsync(consumerIdentifier.Stream);
        await _redis.GetDatabase().StreamCreateConsumerGroupAsync(
            consumerIdentifier.Stream,
            consumerIdentifier.Group,
            StreamPosition.NewMessages,
            createStream
        );
    }

    public async void Start()
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

            await InitStream(consumerIdentifier);

            await Task.Run(() => { handler.RunConsume(_cancellationTokenSource.Token); }, _cancellationTokenSource.Token);

            Logger.Info($"Queue [{contentType.FullName}] start consume");
        }

        _isStart = true;
    }

    public void Stop()
    {
        if (!_isStart)
            return;

        _cancellationTokenSource.Cancel();
    }
}
