// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using Confluent.Kafka;
using eV.Module.EasyLog;
using StackExchange.Redis;
namespace eV.Module.Cluster;

internal static class DefaultSetting
{
    public const int CommunicationPipelineNumber = 8;

    public static readonly ConfigurationOptions RedisOption = new();
    public static KeyValuePair<ProducerConfig, ConsumerConfig> KafkaOption { get; set; } = new();

    public static bool SendAction(string sessionId, byte[] _)
    {
        Logger.Error($"{sessionId}");
        return false;
    }
    public static void SendGroupAction(string groupId, byte[] _)
    {
        Logger.Error($"{groupId}");
    }

    public static void SendBroadcastAction(byte[] _)
    {

    }
}
