// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Module.Queue.Kafka;
using eV.Module.Storage.Redis;
using StackExchange.Redis;
namespace eV.Framework.Server.Base;

public class ServiceBase
{
    private readonly KafkaManger _kafka = KafkaManger.Instance;
    private readonly RedisManager _redis = RedisManager.Instance;
    public IDatabase? GetRedis(string name)
    {
        return _redis.GetRedis(name);
    }

    public Kafka<string, object>? GetKakfa(string name)
    {
        return _kafka.GetKafka(name);
    }
}
