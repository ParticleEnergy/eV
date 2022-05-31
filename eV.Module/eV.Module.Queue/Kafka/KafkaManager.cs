// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using Confluent.Kafka;
using eV.Module.EasyLog;
using eV.Module.Queue.Kafka.Handler;
using eV.Module.Queue.Kafka.Serializer;
namespace eV.Module.Queue.Kafka;

public class KafkaManger<TKey, TValue>
{
    private readonly Dictionary<string, Kafka<TKey, TValue>> _kafka;
    private readonly ErrorHandler<TKey, TValue> _errorHandler;
    private readonly LogHandler<TKey, TValue> _logHandler;

    private bool _isStart;

    public static KafkaManger<TKey, TValue> Instance
    {
        get;
    } = new();


    public KafkaManger()
    {
        _errorHandler = new ErrorHandler<TKey, TValue>();
        _logHandler = new LogHandler<TKey, TValue>();
        _kafka = new Dictionary<string, Kafka<TKey, TValue>>();
        _isStart = false;
    }

    public void Start(Dictionary<string, KeyValuePair<ProducerConfig, ConsumerConfig>> kafkaConfigs)
    {
        if (_isStart)
            return;
        _isStart = true;
        foreach ((string name, KeyValuePair<ProducerConfig, ConsumerConfig> config)in kafkaConfigs)
        {
            Kafka<TKey, TValue> kafka = new(CreateProducer(config.Key), config.Value, CreateConsumer);
            _kafka[name] = kafka;
            Logger.Info($"Kafka [{name}] connected success");
        }
    }

    public Kafka<TKey, TValue>? GetKafka(string name)
    {
        _kafka.TryGetValue(name, out Kafka<TKey, TValue>? kafka);
        return kafka;
    }

    public void Stop()
    {
        foreach ((string name, var kafka) in _kafka)
        {
            kafka.Producer.Flush();
            kafka.Producer.Dispose();
            kafka.CancellationTokenSource.Cancel();
            Logger.Info($"Kafka [{name}] stop");
        }
    }

    private IProducer<TKey, TValue> CreateProducer(ProducerConfig config)
    {
        return new ProducerBuilder<TKey, TValue>(config).SetValueSerializer(new SerializeBson<TValue>()).SetErrorHandler(_errorHandler.ProducerErrorHandler).SetLogHandler(_logHandler.ProducerErrorHandler).Build();
    }

    private IConsumer<TKey, TValue> CreateConsumer(ConsumerConfig config)
    {
        return new ConsumerBuilder<TKey, TValue>(config).SetValueDeserializer(new SerializeBson<TValue>()).SetErrorHandler(_errorHandler.ConsumerErrorHandler).SetLogHandler(_logHandler.ConsumerErrorHandler).Build();
    }
}
