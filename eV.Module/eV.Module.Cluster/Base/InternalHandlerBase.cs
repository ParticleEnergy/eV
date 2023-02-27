// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using System.Text.Json;
using eV.Module.Cluster.Interface;
using eV.Module.EasyLog;
using StackExchange.Redis;

namespace eV.Module.Cluster.Base;

public abstract class InternalHandlerBase<T> : IInternalHandler
{
    protected abstract Task Consume(T data);
    protected int Count { get; set; } = 1;

    private readonly ConsumerIdentifier? _consumerIdentifier;

    public InternalHandlerBase()
    {
        if (CommunicationManager.Instance == null)
        {
            return;
        }

        _consumerIdentifier = CommunicationManager.Instance.GetConsumerIdentifier(typeof(T));
        if (_consumerIdentifier == null)
        {
            Logger.Error($"InternalMessage {typeof(T).Name} not found");
        }
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        if (CommunicationManager.Instance == null)
            return;

        if (_consumerIdentifier == null)
            return;

        while (cancellationToken.IsCancellationRequested)
        {
            var messages = CommunicationManager.Instance.Consume(_consumerIdentifier, Count);

            foreach (StreamEntry message in messages)
            {
                try
                {
                    var data = JsonSerializer.Deserialize<T>(message.Values.First().Value.ToString());
                    if (data == null)
                    {
                        continue;
                    }

                    await Consume(data);
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message, e);
                }
            }
        }
    }
}
