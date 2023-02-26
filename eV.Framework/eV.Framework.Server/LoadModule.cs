// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Framework.Server.Options;
using eV.Framework.Server.Utils;
using eV.Module.Storage.Mongo;
using eV.Module.Storage.Redis;
using StackExchange.Redis;

namespace eV.Framework.Server;

public static class LoadModule
{
    public static void Start()
    {
        if (Configure.Instance.RedisOption == null)
            return;
        Dictionary<string, ConfigurationOptions> configs = new();

        foreach ((string name, RedisOption option) in Configure.Instance.RedisOption)
            configs[name] = ConfigUtils.GetRedisConfig(option);

        RedisManager.Instance.Start(configs);


        if (Configure.Instance.MongodbOption != null)
            MongodbManager.Instance.Start(Configure.Instance.MongodbOption);
    }

    public static void Stop()
    {
        RedisManager.Instance.Stop();
    }
}
