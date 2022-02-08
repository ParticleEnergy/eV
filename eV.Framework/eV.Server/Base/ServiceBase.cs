// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.


using eV.Server.Storage;
using log4net;
using StackExchange.Redis;
namespace eV.Server.Base
{
    public class ServiceBase
    {
        protected readonly ILog Logger = LogManager.GetLogger(DefaultSetting.LoggerName);

        private readonly RedisManager _redis = RedisManager.Instance;
        public IDatabase? GetRedis(string name)
        {
            return _redis.GetRedis(name);
        }
    }
}
