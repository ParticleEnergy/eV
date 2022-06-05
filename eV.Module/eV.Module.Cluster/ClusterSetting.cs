// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using Confluent.Kafka;
using StackExchange.Redis;
namespace eV.Module.Cluster;

public class ClusterSetting
{
    public string ClusterName { get; set; } = DefaultSetting.ClusterName;
    public int ConsumeSendPipelineNumber { get; set; } = DefaultSetting.ConsumeSendPipelineNumber;
    public int ConsumeSendGroupPipelineNumber { get; set; } = DefaultSetting.ConsumeSendGroupPipelineNumber;
    public int ConsumeSendBroadcastPipelineNumber { get; set; } = DefaultSetting.ConsumeSendBroadcastPipelineNumber;

    public ConfigurationOptions RedisOption { get; set; } = DefaultSetting.RedisOption;
    public KeyValuePair<ProducerConfig, ConsumerConfig> KafkaOption { get; set; } = DefaultSetting.KafkaOption;

    public Func<string, byte[], bool>? SendAction { get; set; } = DefaultSetting.SendAction;
    public Action<string, byte[]>? SendGroupAction { get; set; } = DefaultSetting.SendGroupAction;
    public Action<byte[]>? SendBroadcastAction { get; set; } = DefaultSetting.SendBroadcastAction;
}
