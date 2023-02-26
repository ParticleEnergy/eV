// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.EasyLog;
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

    public static RedisManager Instance { get; } = new();

    public async Task Start(Dictionary<string, ConfigurationOptions> redisOptions)
    {
        if (_isStart)
            return;
        _isStart = true;

        foreach ((string name, ConfigurationOptions option) in redisOptions)
            try
            {
                ConnectionMultiplexer conn = await ConnectionMultiplexer.ConnectAsync(option);
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

    public async void Stop()
    {
        foreach ((string? name, ConnectionMultiplexer? connectionMultiplexer) in _redisConnection)
            try
            {
                await connectionMultiplexer.CloseAsync();
                Logger.Info($"Redis [{name}] stop");
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
    }

    public IDatabase? GetRedis(string name)
    {
        return _redisConnection.TryGetValue(name, out ConnectionMultiplexer? connection)
            ? connection.GetDatabase()
            : null;
    }

    public ConnectionMultiplexer? GetRedisConnection(string name)
    {
        return _redisConnection.TryGetValue(name, out ConnectionMultiplexer? connection) ? connection : null;
    }
}
