// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using Confluent.Kafka;
using eV.Module.EasyLog;
using eV.Module.Queue.Kafka.Handler;
using eV.Module.Queue.Kafka.Serializer;
namespace eV.Module.Queue.Kafka;

public class KafkaManger
{
    private readonly Dictionary<string, Kafka<string, object>> _kafka;
    private bool _isStart;

    public static KafkaManger Instance
    {
        get;
    } = new();

    private KafkaManger()
    {
        _kafka = new Dictionary<string, Kafka<string, object>>();
        _isStart = false;
    }

    public void Start(Dictionary<string, KeyValuePair<ProducerConfig, ConsumerConfig>> kafkaConfigs)
    {
        if (_isStart)
            return;
        _isStart = true;

        foreach ((string name, (ProducerConfig? producerConfig, ConsumerConfig? consumerConfig))in kafkaConfigs)
            try
            {
                Kafka<string, object> kafka = new(CreateProducer(producerConfig), consumerConfig, CreateConsumer);
                _kafka[name] = kafka;
                Logger.Info($"Kafka [{name}] connected success");
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
    }

    public void Stop()
    {
        foreach ((string name, var kafka) in _kafka)
        {
            kafka.CancellationTokenSource.Cancel();
            kafka.Producer.Flush();
            kafka.Producer.Dispose();
            Logger.Info($"Kafka [{name}] stop");
        }
    }

    private static IProducer<string, object> CreateProducer(ProducerConfig config)
    {
        return new ProducerBuilder<string, object>(config).SetErrorHandler(ErrorHandler.ProducerErrorHandler).SetLogHandler(LogHandler.ProducerErrorHandler).SetValueSerializer(new SerializeBson<object>()).Build();
    }

    private static IConsumer<string, object> CreateConsumer(ConsumerConfig config)
    {
        return new ConsumerBuilder<string, object>(config).SetErrorHandler(ErrorHandler.ConsumerErrorHandler).SetLogHandler(LogHandler.ConsumerErrorHandler).SetValueDeserializer(new SerializeBson<object>()).Build();
    }

    public Kafka<string, object>? GetKafka(string name)
    {
        _kafka.TryGetValue(name, out Kafka<string, object>? kafka);
        return kafka;
    }
}
