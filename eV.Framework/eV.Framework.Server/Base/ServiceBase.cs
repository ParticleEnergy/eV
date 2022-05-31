// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Module.Cluster;
using eV.Module.Queue.Kafka;
using eV.Module.Storage.Redis;
using StackExchange.Redis;
namespace eV.Framework.Server.Base;

public class ServiceBase
{
    private readonly RedisManager _redis = RedisManager.Instance;
    private readonly KafkaManger<string, object> _kafka = KafkaManger<string, object>.Instance;
    public IDatabase? GetRedis(string name)
    {
        return name == ClusterDefine.Redis ? null : _redis.GetRedis(name);
    }

    public Kafka<string, object>? GetKakfa(string name)
    {
        return name == ClusterDefine.Kafka ? null : _kafka.GetKafka(name);
    }
}
