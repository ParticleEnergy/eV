// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Storage.Redis;
using StackExchange.Redis;

namespace eV.Framework.Server.Base;

public class ServiceBase
{
    private readonly RedisManager _redis = RedisManager.Instance;

    public IDatabase? GetRedis(string name)
    {
        if (!new List<string> { RedisNameReservedWord.QueueInstance, RedisNameReservedWord.ClusterInstance }.Contains(name)) return _redis.GetRedis(name);

        Module.EasyLog.Logger.Warn($"{name} is a reserved word");
        return null;
    }
}
