// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using System.Text.Json;
using eV.Module.EasyLog;
using eV.Module.Queue.Interface;
using StackExchange.Redis;

namespace eV.Module.Queue.Base;

public abstract class QueueBase<T> : IQueueHandler
{
    private readonly ConsumerIdentifier? _consumerIdentifier;

    protected RedisValue Position { get; set; } = ">";
    protected int Count { get; set; } = 1;
    protected bool AutoAck { get; set; } = false;

    protected abstract Task<bool> Consume(T data);

    public QueueBase()
    {
        if (MessageProcessor.Instance == null)
        {
            return;
        }

        _consumerIdentifier = MessageProcessor.Instance.GetConsumerIdentifier(typeof(T));
        if (_consumerIdentifier == null)
        {
            Logger.Error($"QueueMessage {typeof(T).Name} not found");
        }
    }

    public async Task RunConsume(CancellationToken cancellationToken)
    {
        if (MessageProcessor.Instance == null)
            return;

        if (_consumerIdentifier == null)
            return;

        while (cancellationToken.IsCancellationRequested)
        {
            var messages = MessageProcessor.Instance.Consume(
                _consumerIdentifier,
                Position,
                Count,
                AutoAck
            );

            foreach (StreamEntry message in messages)
            {
                try
                {
                    var data = JsonSerializer.Deserialize<T>(message.Values.First().Value.ToString());
                    if (data == null)
                    {
                        if (!AutoAck)
                            MessageProcessor.Instance.Acknowledge(_consumerIdentifier, message.Id);
                        continue;
                    }

                    if (await Consume(data) && !AutoAck)
                    {
                        MessageProcessor.Instance.Acknowledge(_consumerIdentifier, message.Id);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message, e);
                }
            }
        }
    }
}
