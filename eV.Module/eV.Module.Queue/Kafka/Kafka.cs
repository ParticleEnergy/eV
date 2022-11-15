// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using Confluent.Kafka;
using Confluent.Kafka.Admin;
using eV.Module.EasyLog;

namespace eV.Module.Queue.Kafka;

public class Kafka<TKey, TValue>
{
    public IProducer<TKey, TValue> Producer { get; }
    private readonly IAdminClient _adminClient;

    public CancellationTokenSource CancellationTokenSource { get; }

    private readonly Func<ConsumerConfig, IConsumer<TKey, TValue>> _createConsumer;
    private readonly ConsumerConfig _consumerConfig;
    private readonly CancellationToken _cancellationToken;

    public Kafka(IAdminClient adminClient, IProducer<TKey, TValue> producer, ConsumerConfig consumerConfig,
        Func<ConsumerConfig, IConsumer<TKey, TValue>> createConsumer)
    {
        _adminClient = adminClient;

        Producer = producer;
        _createConsumer = createConsumer;
        _consumerConfig = consumerConfig;
        CancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = CancellationTokenSource.Token;
    }

    public void Produce(string topic, TKey messageKey, TValue messageValue,
        Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null)
    {
        Producer.Produce(
            topic,
            new Message<TKey, TValue> { Key = messageKey, Value = messageValue },
            deliveryHandler
        );
    }

    public void Produce(string topic, int partition, TKey messageKey, TValue messageValue,
        Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null)
    {
        Producer.Produce(
            new TopicPartition(topic, new Partition(partition)),
            new Message<TKey, TValue> { Key = messageKey, Value = messageValue },
            deliveryHandler
        );
    }

    public Task<DeliveryResult<TKey, TValue>> ProduceAsync(string topic, TKey messageKey, TValue messageValue)
    {
        return Producer.ProduceAsync(
            topic,
            new Message<TKey, TValue> { Key = messageKey, Value = messageValue },
            _cancellationToken
        );
    }

    public Task<DeliveryResult<TKey, TValue>> ProduceAsync(string topic, int partition, TKey messageKey,
        TValue messageValue)
    {
        return Producer.ProduceAsync(
            new TopicPartition(topic, new Partition(partition)),
            new Message<TKey, TValue> { Key = messageKey, Value = messageValue },
            _cancellationToken
        );
    }

    public void Consume(ConsumerConfig config, Action<IConsumer<TKey, TValue>> preparation,
        Func<ConsumeResult<TKey, TValue>, bool> consume, Action<IConsumer<TKey, TValue>, bool>? result = null)
    {
        config.BootstrapServers = _consumerConfig.BootstrapServers;
        config.AllowAutoCreateTopics = _consumerConfig.AllowAutoCreateTopics;

        config.SaslMechanism = _consumerConfig.SaslMechanism;
        config.SecurityProtocol = _consumerConfig.SecurityProtocol;
        config.SaslUsername = _consumerConfig.SaslUsername;
        config.SaslPassword = _consumerConfig.SaslPassword;

        config.HeartbeatIntervalMs = _consumerConfig.HeartbeatIntervalMs;
        config.SessionTimeoutMs = _consumerConfig.SessionTimeoutMs;

        config.AutoOffsetReset = _consumerConfig.AutoOffsetReset;
        config.EnablePartitionEof = _consumerConfig.EnablePartitionEof;

        config.SocketKeepaliveEnable = _consumerConfig.SocketKeepaliveEnable;
        config.SocketTimeoutMs = _consumerConfig.SocketTimeoutMs;
        config.SocketReceiveBufferBytes = _consumerConfig.SocketReceiveBufferBytes;
        config.SocketSendBufferBytes = _consumerConfig.SocketSendBufferBytes;

        ConsumeAction(_createConsumer(config), preparation, consume, result, config.EnableAutoCommit ?? true);
    }

    public void Consume(Action<IConsumer<TKey, TValue>> preparation, Func<ConsumeResult<TKey, TValue>?, bool> consume,
        Action<IConsumer<TKey, TValue>, bool>? result = null)
    {
        ConsumeAction(_createConsumer(_consumerConfig), preparation, consume, result,
            _consumerConfig.EnableAutoCommit ?? true);
    }

    private void ConsumeAction(IConsumer<TKey, TValue> consumer, Action<IConsumer<TKey, TValue>> preparation,
        Func<ConsumeResult<TKey, TValue>, bool> consume, Action<IConsumer<TKey, TValue>, bool>? result, bool autoCommit)
    {
        preparation(consumer);

        while (true)
        {
            try
            {
                var data = consumer.Consume(_cancellationToken);
                if (_cancellationToken.IsCancellationRequested)
                {
                    if (!autoCommit && data != null)
                    {
                        consumer.Commit(data);
                    }

                    consumer.Close();
                    consumer.Dispose();
                    return;
                }

                if (data.IsPartitionEOF)
                {
                    Logger.Info(
                        $"Kafka Reached end of topic {data.Topic}, partition {data.Partition}, offset {data.Offset}.");
                    continue;
                }

                bool flag = consume.Invoke(data);
                result?.Invoke(consumer, flag);
            }
            catch (ConsumeException e)
            {
                if (e.Error.Code == ErrorCode.UnknownTopicOrPart)
                {
                    try
                    {
                        _adminClient.CreateTopicsAsync(new[] { new TopicSpecification { Name = e.ConsumerRecord.Topic } });
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception.Message, exception);
                    }

                    continue;
                }

                Logger.Error($"Kafka Error code:{e.Error.Code} reason: {e.Error.Reason}");
            }
        }
    }
}
