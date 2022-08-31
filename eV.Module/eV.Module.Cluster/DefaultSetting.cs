// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using Confluent.Kafka;
using eV.Module.EasyLog;
using StackExchange.Redis;
namespace eV.Module.Cluster;

internal static class DefaultSetting
{
    public const string ClusterName = "eV.Cluster";

    public const int ConsumeSendPipelineNumber = 4;
    public const int ConsumeSendGroupPipelineNumber = 2;
    public const int ConsumeSendBroadcastPipelineNumber = 1;

    public static readonly ConfigurationOptions RedisOption = new();
    public static KeyValuePair<ProducerConfig, ConsumerConfig> KafkaOption { get; set; } = new();
}
