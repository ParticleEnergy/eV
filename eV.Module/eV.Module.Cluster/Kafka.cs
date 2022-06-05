// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using Confluent.Kafka;
using Confluent.Kafka.Admin;
using eV.Module.EasyLog;
namespace eV.Module.Cluster;

public class Kafka
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CancellationToken _cancellationToken;

    private readonly IAdminClient _adminClient;
    private readonly IProducer<string, byte[]> _producer;
    private readonly ConsumerConfig _consumerConfig;

    private readonly string _clusterName;
    private readonly string _nodeName;

    public Kafka(string clusterName, string nodeName, KeyValuePair<ProducerConfig, ConsumerConfig> kafkaOption)
    {
        _clusterName = clusterName;
        _nodeName = nodeName;

        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;

        (ProducerConfig? producerConfig, ConsumerConfig? consumerConfig) = kafkaOption;

        _adminClient = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = producerConfig.BootstrapServers
        }).Build();

        _consumerConfig = consumerConfig;
        _consumerConfig.ClientId = $"eV.Cluster-{_consumerConfig.ClientId}";

        _producer = new ProducerBuilder<string, byte[]>(producerConfig).SetErrorHandler(
            delegate(IProducer<string, byte[]> _, Error error)
            {
                Logger.Error($"Cluster [{_clusterName}] [{_nodeName}] Kafka Error code:{error.Code} reason: {error.Reason}");
            }
        ).SetLogHandler(delegate(IProducer<string, byte[]> _, LogMessage message) { Log(message); }).Build();
    }

    public void Produce(string topic, string key, byte[] value)
    {
        _producer.Produce(topic, new Message<string, byte[]>
        {
            Key = key, Value = value
        });
    }


    public void Consume(string groupId, string topic, Action<ConsumeResult<string, byte[]>> action)
    {
        IConsumer<string, byte[]> consumer = CreateConsumer(groupId);

        consumer.Subscribe(topic);

        while (true)
        {
            try
            {
                var data = consumer.Consume(_cancellationToken);
                if (_cancellationToken.IsCancellationRequested)
                {
                    consumer.Close();
                    consumer.Dispose();
                    return;
                }

                if (data.IsPartitionEOF)
                {
                    continue;
                }

                action.Invoke(data);
            }
            catch (ConsumeException e)
            {
                if (e.Error.Code == ErrorCode.UnknownTopicOrPart)
                {
                    try
                    {
                        _adminClient.CreateTopicsAsync(new[]
                        {
                            new TopicSpecification
                            {
                                Name = e.ConsumerRecord.Topic
                            }
                        });
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception.Message, exception);
                    }
                    continue;
                }
                Logger.Error($"Cluster [{_clusterName}] [{_nodeName}] Kafka Error code:{e.Error.Code} reason: {e.Error.Reason}");
            }
        }
    }

    private IConsumer<string, byte[]> CreateConsumer(string groupId)
    {
        ConsumerConfig consumerConfig = new()
        {
            BootstrapServers = _consumerConfig.BootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            AllowAutoCreateTopics = true,
            EnableAutoCommit = true,
            EnablePartitionEof = true,
            SocketKeepaliveEnable = true,
            SaslMechanism = _consumerConfig.SaslMechanism,
            SecurityProtocol = _consumerConfig.SecurityProtocol,
            SaslUsername = _consumerConfig.SaslUsername,
            SaslPassword = _consumerConfig.SaslPassword,
            SocketTimeoutMs = _consumerConfig.SocketTimeoutMs,
            SocketReceiveBufferBytes = _consumerConfig.SocketReceiveBufferBytes,
            SocketSendBufferBytes = _consumerConfig.SocketSendBufferBytes,
            HeartbeatIntervalMs = _consumerConfig.HeartbeatIntervalMs,
            SessionTimeoutMs = _consumerConfig.SessionTimeoutMs
        };

        return new ConsumerBuilder<string, byte[]>(consumerConfig).SetErrorHandler(
            delegate(IConsumer<string, byte[]> _, Error error)
            {
                Logger.Error($"Cluster [{_clusterName}] [{_nodeName}] Kafka Error code:{error.Code} reason: {error.Reason}");
            }
        ).SetLogHandler(delegate(IConsumer<string, byte[]> _, LogMessage message) { Log(message); }).Build();
    }

    private void Log(LogMessage logMessage)
    {
        string message = $"Cluster [{_clusterName}] [{_nodeName}] Kafka {logMessage.Message}";
        switch (logMessage.Level)
        {
            case SyslogLevel.Emergency:
                Logger.Fatal(message);
                break;
            case SyslogLevel.Alert:
                Logger.Warn(message);
                break;
            case SyslogLevel.Critical:
                Logger.Debug(message);
                break;
            case SyslogLevel.Error:
                Logger.Error(message);
                break;
            case SyslogLevel.Warning:
                Logger.Warn(message);
                break;
            case SyslogLevel.Notice:
                Logger.Debug(message);
                break;
            case SyslogLevel.Info:
                Logger.Info(message);
                break;
            case SyslogLevel.Debug:
                Logger.Debug(message);
                break;
            default:
                Logger.Info(message);
                break;
        }
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }
}
