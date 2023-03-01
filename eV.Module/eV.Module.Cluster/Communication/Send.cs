// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Module.Cluster.Interface;
using eV.Module.EasyLog;
using StackExchange.Redis;

namespace eV.Module.Cluster.Communication;

public class Send : IInternalHandler
{
    public Dictionary<int, ConsumerIdentifier> ConsumerIdentifiers { get; } = new();

    private readonly Func<string, byte[], bool> _action;
    private readonly int _batchProcessingQuantity;

    public Send(
        string clusterId,
        Func<string, byte[], bool> action,
        int batchProcessingQuantity = 1
    )
    {
        _batchProcessingQuantity = batchProcessingQuantity;
        _action = action;

        for (int i = 0; i < batchProcessingQuantity; i++)
        {
            ConsumerIdentifiers[i] = new ConsumerIdentifier(clusterId, CommunicationStream.GetSendStream(i));
        }
    }


    public Task Run(CancellationToken cancellationToken)
    {
        for (int i = 0; i < _batchProcessingQuantity; i++)
        {
            if (!ConsumerIdentifiers.ContainsKey(i))
                continue;

            if (!ConsumerIdentifiers.TryGetValue(i, out ConsumerIdentifier? consumerIdentifier))
                continue;

            Task.Run(() => { Action(consumerIdentifier, cancellationToken); }, cancellationToken);
        }

        return Task.CompletedTask;
    }

    private async void Action(ConsumerIdentifier consumerIdentifier, CancellationToken cancellationToken)
    {
        if (CommunicationManager.Instance == null)
            return;

        string stream = consumerIdentifier.GetStream(CommunicationManager.Instance.NodeId);
        string group = consumerIdentifier.GetGroup(CommunicationManager.Instance.NodeId);
        string consumer = consumerIdentifier.GetConsumer(CommunicationManager.Instance.NodeId);

        while (!cancellationToken.IsCancellationRequested)
        {
            var messages = CommunicationManager.Instance.Consume(stream, group, consumer);

            if (messages.Length <=0 )
                continue;

            List<RedisValue> deleteIds = new();
            foreach (StreamEntry message in messages)
            {
                try
                {
                    string? sessionId = message[CommunicationStream.GetSessionIdKey()];
                    string? body = message[CommunicationStream.GetBodyKey()];

                    if (sessionId == null || body == null) continue;

                    byte[] data = Convert.FromBase64String(body);
                    _action.Invoke(sessionId, data);
                    deleteIds.Add(message.Id);
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message, e);
                }
            }
            if (deleteIds.Count > 0)
                await CommunicationManager.Instance.DeleteMessage(stream, deleteIds.ToArray());
        }
    }
}
