// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Module.Cluster.Interface;
using eV.Module.EasyLog;
using StackExchange.Redis;

namespace eV.Module.Cluster.Communication;

public class Send : IInternalHandler
{
    public  Dictionary<int, ConsumerIdentifier> ConsumerIdentifiers { get; } = new();

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
                    string? sessionId = message[CommunicationStream.GetSessionIdKey()];
                    string? body = message[CommunicationStream.GetBodyKey()];

                    if (sessionId == null || body == null) continue;

                    byte[] data = Convert.FromBase64String(body);
                    _action.Invoke(sessionId, data);
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message, e);
                }
            }
        }
    }
}
