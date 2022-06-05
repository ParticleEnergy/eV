// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using Confluent.Kafka;
using eV.Framework.Server.Utils;
using eV.Module.Queue.Kafka;
using eV.Module.Storage.Mongo;
using eV.Module.Storage.Redis;
using StackExchange.Redis;
namespace eV.Framework.Server;

public static class LoadModule
{
    public static void Run()
    {
        Redis();
        Mongodb();
        Kafka();
    }

    public static void Stop()
    {
        RedisManager.Instance.Stop();
        KafkaManger.Instance.Stop();
    }

    #region Storage
    private static void Redis()
    {
        if (Configure.Instance.RedisOption == null)
            return;
        Dictionary<string, ConfigurationOptions> configs = new();

        foreach ((string name, var option) in Configure.Instance.RedisOption)
            configs[name] = ConfigUtils.GetRedisConfig(option);

        RedisManager.Instance.Start(configs);
    }

    private static void Mongodb()
    {
        if (Configure.Instance.MongodbOption != null)
            MongodbManager.Instance.Start(Configure.Instance.MongodbOption);
    }
    #endregion

    #region Queue
    private static void Kafka()
    {
        if (Configure.Instance.KafkaOption == null)
            return;

        Dictionary<string, KeyValuePair<ProducerConfig, ConsumerConfig>> configs = new();
        foreach ((string name, var option) in Configure.Instance.KafkaOption)
        {
            configs[name] = ConfigUtils.GetKafkaConfig(option);
            configs[name].Value.ClientId = $"{name}-{configs[name].Value.ClientId}";
        }

        KafkaManger.Instance.Start(configs);
    }
    #endregion
}
