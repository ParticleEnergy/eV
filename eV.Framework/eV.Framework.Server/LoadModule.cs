// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using Confluent.Kafka;
using eV.Framework.Server.Options;
using eV.Framework.Server.Utils;
using eV.Module.Queue.Kafka;
using eV.Module.Storage.Mongo;
using eV.Module.Storage.Redis;
using StackExchange.Redis;
namespace eV.Framework.Server;

public static class LoadModule
{
    public static void StartAll()
    {
        RedisStart();
        MongodbStart();
        KafkaStart();
    }

    public static void StopAll()
    {
        RedisStop();
        KafkaStop();
    }

    #region Queue
    public static void KafkaStart()
    {
        if (Configure.Instance.KafkaOption == null)
            return;

        Dictionary<string, KeyValuePair<ProducerConfig, ConsumerConfig>> configs = new();
        foreach ((string name, KafkaOption option) in Configure.Instance.KafkaOption)
        {
            configs[name] = ConfigUtils.GetKafkaConfig(option);
            configs[name].Value.ClientId = $"{name}-{configs[name].Value.ClientId}";
        }

        KafkaManger.Instance.Start(configs);
    }

    public static void KafkaStop()
    {
        KafkaManger.Instance.Stop();
    }

    #endregion

    #region Storage
    public static void RedisStart()
    {
        if (Configure.Instance.RedisOption == null)
            return;
        Dictionary<string, ConfigurationOptions> configs = new();

        foreach ((string name, RedisOption option) in Configure.Instance.RedisOption)
            configs[name] = ConfigUtils.GetRedisConfig(option);

        RedisManager.Instance.Start(configs);
    }

    public static void RedisStop()
    {
        RedisManager.Instance.Stop();
    }

    public static void MongodbStart()
    {
        if (Configure.Instance.MongodbOption != null)
            MongodbManager.Instance.Start(Configure.Instance.MongodbOption);
    }
    #endregion
}
