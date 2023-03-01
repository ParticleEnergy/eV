// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using System.Text.Json;
using eV.Module.EasyLog;
using eV.Module.Queue.Interface;
using StackExchange.Redis;

namespace eV.Module.Queue.Base;

public abstract class QueueBase<T> : IQueueHandler
{
    protected RedisValue Position { get; set; } = ">";
    protected int Count { get; set; } = 1;
    protected bool AutoAck { get; set; } = false;

    protected abstract Task<bool> Consume(T data);

    public async Task RunConsume(CancellationToken cancellationToken)
    {
        if (MessageProcessor.Instance == null)
            return;

        ConsumerIdentifier? consumerIdentifier = MessageProcessor.Instance.GetConsumerIdentifier(typeof(T));
        if (consumerIdentifier == null)
        {
            Logger.Error($"QueueMessage {typeof(T).Name} not found");
            return;
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            var messages = MessageProcessor.Instance.Consume(
                consumerIdentifier,
                Position,
                Count,
                AutoAck
            );
            if (messages.Length <= 0)
                continue;

            List<RedisValue> deleteIds = new();
            foreach (StreamEntry message in messages)
            {
                try
                {
                    var data = JsonSerializer.Deserialize<T>(message.Values.First().Value.ToString());
                    if (data == null)
                    {
                        if (!AutoAck)
                        {
                            if (MessageProcessor.Instance.AckMessage(consumerIdentifier, message.Id))
                            {
                                deleteIds.Add(message.Id);
                            }
                        }

                        continue;
                    }

                    if (!await Consume(data)) continue;

                    if (!AutoAck)
                    {
                        MessageProcessor.Instance.AckMessage(consumerIdentifier, message.Id);
                    }

                    deleteIds.Add(message.Id);
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message, e);
                }
            }

            if (deleteIds.Count > 0)
                await MessageProcessor.Instance.DeleteMessage(consumerIdentifier, deleteIds.ToArray());
        }
    }
}
