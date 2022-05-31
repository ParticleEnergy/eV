// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using Confluent.Kafka;
namespace eV.Module.Queue.Kafka;

public class Kafka<TKey, TValue>
{
    private const string GroupId = "eV.Module.Kafka";

    public IProducer<TKey, TValue> Producer
    {
        get;
    }

    public CancellationTokenSource CancellationTokenSource { get; }

    private readonly Func<ConsumerConfig, IConsumer<TKey, TValue>> _createConsumer;
    private readonly ConsumerConfig _consumerConfig;
    private readonly CancellationToken _cancellationToken;
    public Kafka(IProducer<TKey, TValue> producer, ConsumerConfig consumerConfig, Func<ConsumerConfig, IConsumer<TKey, TValue>> createConsumer)
    {
        Producer = producer;
        _createConsumer = createConsumer;
        _consumerConfig = consumerConfig;
        CancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = CancellationTokenSource.Token;
    }

    public void Produce(string topic, Message<TKey, TValue> message, Action<DeliveryReport<TKey, TValue>>? deliveryHandler)
    {
        Producer.Produce(topic, message, deliveryHandler);
    }

    public void Produce(string topic, int partition, Message<TKey, TValue> message, Action<DeliveryReport<TKey, TValue>>? deliveryHandler)
    {
        Producer.Produce(new TopicPartition(topic, new Partition(partition)), message, deliveryHandler);
    }

    public Task<DeliveryResult<TKey, TValue>> ProduceAsync(string topic, Message<TKey, TValue> message)
    {
        return Producer.ProduceAsync(topic, message, _cancellationToken);
    }

    public Task<DeliveryResult<TKey, TValue>> ProduceAsync(string topic, int partition, Message<TKey, TValue> message)
    {
        return Producer.ProduceAsync(new TopicPartition(topic, new Partition(partition)), message, _cancellationToken);
    }

    public IConsumer<TKey, TValue> GetConsumer(string? groupId)
    {
        _consumerConfig.GroupId = groupId is null or "" ? GroupId : groupId;
        return _createConsumer(_consumerConfig);
    }

    public bool Consume(IConsumer<TKey, TValue> consumer, Func<ConsumeResult<TKey, TValue>?, CancellationTokenSource, bool> action, bool autoCommit = true)
    {
        var result = consumer.Consume(_cancellationToken);
        if (_cancellationToken.IsCancellationRequested)
            return false;
        CancellationTokenSource cancellationTokenSource = new();
        if (!autoCommit && result != null)
        {
            cancellationTokenSource.Token.Register(() =>
            {
                consumer.Commit(result);
            });
        }
        return action.Invoke(result, cancellationTokenSource);
    }
}
