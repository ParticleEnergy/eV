// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using System.Text.Json;
using eV.Module.EasyLog;
using eV.Module.Queue.Interface;
using StackExchange.Redis;

namespace eV.Module.Queue.Base;

public abstract class QueueBase<T> : IQueueHandler
{
    private readonly string _stream;
    private readonly string _group;
    private readonly string _consumer;

    protected RedisValue Position { get; set; } = ">";
    protected int Count { get; set; } = 1;
    protected bool AutoAck { get; set; } = false;

    protected abstract Task<bool> Consume(T data);

    public QueueBase()
    {
        if (MessageProcessor.Instance == null)
        {
            _stream = "";
            _group = "";
            _consumer = "";
            return;
        }

        ConsumerIdentifier? consumerIdentifier = MessageProcessor.Instance.GetConsumerIdentifier(typeof(T));
        if (consumerIdentifier == null)
        {
            _stream = "";
            _group = "";
            _consumer = "";
            Logger.Error($"Message {typeof(T).Name} not found");
            return;
        }

        _stream = consumerIdentifier.Stream;
        _group = consumerIdentifier.Group;
        _consumer = consumerIdentifier.Consumer;
    }

    public async Task RunConsume(CancellationToken cancellationToken)
    {
        if (MessageProcessor.Instance == null)
            return;

        while (cancellationToken.IsCancellationRequested)
        {
            var messages = MessageProcessor.Instance.Consume(
                _stream,
                _group,
                _consumer,
                Position,
                Count,
                AutoAck);

            foreach (StreamEntry message in messages)
            {
                try
                {
                    var data = JsonSerializer.Deserialize<T>(message.Values.First().Value.ToString());
                    if (data == null)
                    {
                        if (!AutoAck)
                            MessageProcessor.Instance.Acknowledge(_stream, _group, message.Id);
                        continue;
                    }

                    if (await Consume(data) && !AutoAck)
                    {
                        MessageProcessor.Instance.Acknowledge(_stream, _group, message.Id);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message, e);
                }
            }
        }
    }

    public async Task<bool> Produce<TValue>(TValue data)
    {
        if (MessageProcessor.Instance == null)
            return false;
        return await MessageProcessor.Instance.Produce(data);
    }
}
