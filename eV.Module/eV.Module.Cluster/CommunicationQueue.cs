// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using Confluent.Kafka;
using eV.Module.Cluster.Interface;
using eV.Module.EasyLog;
namespace eV.Module.Cluster;

public class CommunicationQueue : ICommunicationQueue
{
    // private readonly string _clusterName;
    // private readonly string _nodeName;
    // private event Func<string, byte[], bool>? SendAction;
    // private event Action<string, byte[]>? SendGroupAction;
    // private event Action<byte[]>? SendBroadcastAction;
    //
    // private readonly int _pipelineNumber;
    //
    // private IProducer<string, byte[]>? _producer;
    // private IConsumer<string, byte[]>? _consumerSend;
    // private IConsumer<string, byte[]>? _consumerSendGroup;
    // private IConsumer<string, byte[]>? _consumerSendBroadcast;
    //
    // public readonly string QueueSend;
    // public readonly string QueueSendGroup;
    // public readonly string QueueSendBroadcast;
    // public CommunicationQueue(string clusterName, string nodeName, int pipelineNumber, KeyValuePair<ProducerConfig, ConsumerConfig> kafkaOption)
    // {
    //     _clusterName = clusterName;
    //     _nodeName = nodeName;
    //     _pipelineNumber = pipelineNumber;
    //
    //     QueueSend = string.Format(Define.TopicQueueSend, _clusterName, _nodeName);
    //     QueueSendGroup = string.Format(Define.TopicQueueSendGroup, _clusterName);
    //     QueueSendBroadcast = string.Format(Define.TopicQueueSendBroadcast, _clusterName);
    //
    //     InitKafka(kafkaOption);
    // }
    //
    // public void SendProducer(string sessionId, byte[] data)
    // {
    // }
    //
    // public void SendGroupProducer(string groupId, byte[] data)
    // {
    //     _producer.Produce(QueueSendGroup, new Message<string, byte[]>{Key = groupId, Value = data});
    // }
    //
    // public void SendBroadcastProducer(byte[] data)
    // {
    //     _producer.Produce(QueueSendBroadcast, new Message<string, byte[]>{Value = data});
    // }
    //
    // private void Consumer()
    // {
    //
    // }
    //
    // private void InitKafka(KeyValuePair<ProducerConfig, ConsumerConfig> kafkaOption)
    // {
    //     (ProducerConfig? producerConfig, ConsumerConfig? consumerConfig) = kafkaOption;
    //
    //     _producer = new ProducerBuilder<string, byte[]>(producerConfig).SetErrorHandler(
    //         delegate(IProducer<string, byte[]> _, Error error)
    //         {
    //             Logger.Error($"Cluster [{_clusterName}] [{_nodeName}] Kafka Error code:{error.Code} reason: {error.Reason}");
    //         }
    //     ).SetLogHandler(delegate(IProducer<string, byte[]> _, LogMessage message) { Log(message); }).Build();
    //
    //     _consumerSend = GetConsumerConfig(consumerConfig, _clusterName);
    //     _consumerSendGroup = GetConsumerConfig(consumerConfig, _nodeName);
    //     _consumerSendBroadcast = GetConsumerConfig(consumerConfig, _nodeName);
    // }
    //
    // private IConsumer<string, byte[]> GetConsumerConfig(ConsumerConfig config, string groupId)
    // {
    //     ConsumerConfig consumerConfig = new()
    //     {
    //         AutoOffsetReset = AutoOffsetReset.Earliest,
    //         EnableAutoCommit = true,
    //         EnablePartitionEof = true,
    //         SocketKeepaliveEnable = true,
    //         BootstrapServers = config.BootstrapServers,
    //
    //         SaslMechanism = config.SaslMechanism,
    //         SecurityProtocol = config.SecurityProtocol,
    //         SaslUsername = config.SaslUsername,
    //         SaslPassword = config.SaslPassword,
    //
    //         SocketTimeoutMs = config.SocketTimeoutMs,
    //         SocketReceiveBufferBytes = config.SocketReceiveBufferBytes,
    //         SocketSendBufferBytes = config.SocketSendBufferBytes,
    //
    //         HeartbeatIntervalMs = config.HeartbeatIntervalMs,
    //         SessionTimeoutMs = config.SessionTimeoutMs,
    //
    //         GroupId = groupId
    //     };
    //
    //    return  new ConsumerBuilder<string, byte[]>(consumerConfig).SetErrorHandler(
    //         delegate(IConsumer<string, byte[]> _, Error error)
    //         {
    //             Logger.Error($"Cluster [{_clusterName}] [{_nodeName}] Kafka Error code:{error.Code} reason: {error.Reason}");
    //         }
    //     ).SetLogHandler(delegate(IConsumer<string, byte[]> _, LogMessage message) { Log(message); }).Build();
    // }
    //
    //
    // private void Log(LogMessage logMessage)
    // {
    //     string message = $"Cluster [{_clusterName}] [{_nodeName}] Kafka {logMessage.Message}";
    //     switch (logMessage.Level)
    //     {
    //         case SyslogLevel.Emergency:
    //             Logger.Fatal(message);
    //             break;
    //         case SyslogLevel.Alert:
    //             Logger.Warn(message);
    //             break;
    //         case SyslogLevel.Critical:
    //             Logger.Debug(message);
    //             break;
    //         case SyslogLevel.Error:
    //             Logger.Error(message);
    //             break;
    //         case SyslogLevel.Warning:
    //             Logger.Warn(message);
    //             break;
    //         case SyslogLevel.Notice:
    //             Logger.Debug(message);
    //             break;
    //         case SyslogLevel.Info:
    //             Logger.Info(message);
    //             break;
    //         case SyslogLevel.Debug:
    //             Logger.Debug(message);
    //             break;
    //         default:
    //             Logger.Info(message);
    //             break;
    //     }
    // }
}
