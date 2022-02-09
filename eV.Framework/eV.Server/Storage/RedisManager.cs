// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using eV.EasyLog;
using eV.Server.Options;
using StackExchange.Redis;
namespace eV.Server.Storage
{
    public class RedisManager
    {
        public static RedisManager Instance
        {
            get;
        } = new();

        private readonly Dictionary<string, ConnectionMultiplexer> _redisConnection;
        private bool _isStart;

        private RedisManager()
        {
            _isStart = false;
            _redisConnection = new Dictionary<string, ConnectionMultiplexer>();
        }

        public void Start()
        {
            if (_isStart)
                return;
            _isStart = true;

            if (Configure.Instance.StorageOptions.Redis == null)
                return;

            foreach (var (name, redisOptions) in Configure.Instance.StorageOptions.Redis)
            {
                if (redisOptions.Address == null)
                {
                    Logger.Warn($"Redis {name} address is null");
                    return;
                }
                try
                {
                    var config = GetConfig(redisOptions.Address, redisOptions.Database, redisOptions.User, redisOptions.Password);
                    var conn = ConnectionMultiplexer.Connect(config);
                    if (conn.IsConnected)
                    {
                        _redisConnection.Add(name, conn);
                        Logger.Info($"Redis [{name}] connected success");
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message, e);
                }
            }
        }

        public void Stop()
        {
            foreach (var (name, connectionMultiplexer) in _redisConnection)
            {
                try
                {
                    connectionMultiplexer.Close();
                    Logger.Info($"Redis [{name}] stop");
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message, e);
                }
            }
        }

        private static ConfigurationOptions GetConfig(List<AddressOptions> addressList, int? database, string? user, string? password)
        {
            ConfigurationOptions config = new();
            foreach (var address in addressList)
            {
                config.EndPoints.Add(address.Host, address.Port);
            }
            if (user != null)
            {
                config.User = user;
            }
            if (password != null)
            {
                config.Password = password;
            }
            config.DefaultVersion = new Version(4, 0, 9);
            config.DefaultDatabase = addressList.Count == 1 && database != null ? database : 0;
            config.KeepAlive = 60;
            return config;
        }

        public IDatabase? GetRedis(string name)
        {
            return _redisConnection.TryGetValue(name, out ConnectionMultiplexer? connection) ? connection.GetDatabase() : null;
        }
    }
}
