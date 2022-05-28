// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.EasyLog;
using eV.Module.Storage.Redis.Config;
using StackExchange.Redis;
namespace eV.Module.Storage.Redis;

public class RedisManager
{

    private readonly Dictionary<string, ConnectionMultiplexer> _redisConnection;
    private bool _isStart;

    private RedisManager()
    {
        _isStart = false;
        _redisConnection = new Dictionary<string, ConnectionMultiplexer>();
    }
    public static RedisManager Instance
    {
        get;
    } = new();

    public void Start(Dictionary<string, RedisOptions> redisOptions)
    {
        if (_isStart)
            return;
        _isStart = true;

        foreach ((string name, RedisOptions options) in redisOptions)
            try
            {
                ConfigurationOptions config = GetConfig(options.Address, options.Database, options.User, options.Password);
                ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(config);
                if (!conn.IsConnected)
                    continue;
                _redisConnection.Add(name, conn);
                Logger.Info($"Redis [{name}] connected success");
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
    }

    public void Stop()
    {
        foreach ((string? name, ConnectionMultiplexer? connectionMultiplexer) in _redisConnection)
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

    private static ConfigurationOptions GetConfig(List<RedisAddress> addressList, int? database, string? user, string? password)
    {
        ConfigurationOptions config = new();
        foreach (RedisAddress? address in addressList)
            config.EndPoints.Add(address.Host, address.Port);
        if (user != null)
            config.User = user;
        if (password != null)
            config.Password = password;
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
