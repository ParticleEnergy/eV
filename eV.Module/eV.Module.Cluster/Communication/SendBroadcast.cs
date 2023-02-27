// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Module.Cluster.Interface;
using eV.Module.EasyLog;
using StackExchange.Redis;

namespace eV.Module.Cluster.Communication;

public class SendBroadcast : IInternalHandler
{
    public  Dictionary<int, ConsumerIdentifier> ConsumerIdentifiers { get; } = new();

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


    public async Task Run(CancellationToken cancellationToken)
    {
        for (int i = 0; i < _batchProcessingQuantity; i++)
        {
            if (!ConsumerIdentifiers.ContainsKey(i))
                continue;

            if (!ConsumerIdentifiers.TryGetValue(i, out ConsumerIdentifier? consumerIdentifier))
                continue;

            await Task.Run(() => { Action(consumerIdentifier, cancellationToken); }, cancellationToken);
        }
    }

    public void Action(ConsumerIdentifier consumerIdentifier, CancellationToken cancellationToken)
    {
        if (CommunicationManager.Instance == null)
            return;

        while (cancellationToken.IsCancellationRequested)
        {
            var messages = CommunicationManager.Instance.Consume(consumerIdentifier, _batchProcessingQuantity);
            foreach (StreamEntry message in messages)
            {
                try
                {
                    byte[] data = Convert.FromBase64String(message.Values.First().Value.ToString());
                    _action.Invoke(data);
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message, e);
                }
            }
        }
    }
}
