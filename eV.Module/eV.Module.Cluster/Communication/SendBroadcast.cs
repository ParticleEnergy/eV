// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Module.Cluster.Interface;
using eV.Module.EasyLog;
using StackExchange.Redis;

namespace eV.Module.Cluster.Communication;

public class SendBroadcast : IInternalHandler
{
    public Dictionary<int, ConsumerIdentifier> ConsumerIdentifiers { get; } = new();

    private readonly Action<byte[]> _action;
    private readonly int _batchProcessingQuantity;

    public SendBroadcast(
        string clusterId,
        Action<byte[]> action,
        int batchProcessingQuantity = 1
    )
    {
        _batchProcessingQuantity = batchProcessingQuantity;
        _action = action;

        for (int i = 0; i < batchProcessingQuantity; i++)
        {
            ConsumerIdentifiers[i] = new ConsumerIdentifier(clusterId, CommunicationStream.GetSendBroadcastStream(i));
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
                    byte[] data = Convert.FromBase64String(message.Values.First().Value.ToString());
                    _action.Invoke(data);
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
