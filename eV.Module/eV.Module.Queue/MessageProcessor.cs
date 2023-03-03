// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using System.Text.Json;
using eV.Module.EasyLog;
using StackExchange.Redis;

namespace eV.Module.Queue;

public class MessageProcessor
{
    private readonly IDatabase _redisInstance;
    private readonly Dictionary<Type, ConsumerIdentifier> _consumerIdentifiers;

    public static MessageProcessor? Instance { get; private set; }

    private MessageProcessor(IConnectionMultiplexer redis, Dictionary<Type, ConsumerIdentifier> consumerIdentifiers)
    {
        _redisInstance = redis.GetDatabase();
        _consumerIdentifiers = consumerIdentifiers;
    }

    public static void InitMessageProcessor(ConnectionMultiplexer redis, Dictionary<Type, ConsumerIdentifier> consumerIdentifiers)
    {
        if (Instance != null)
            return;

        Instance = new MessageProcessor(redis, consumerIdentifiers);
    }

    public async Task<bool> Produce<TValue>(TValue data)
    {
        try
        {
            var type = typeof(TValue);
            if (!_consumerIdentifiers.ContainsKey(type))
            {
                return false;
            }

            if (!_consumerIdentifiers.TryGetValue(type, out ConsumerIdentifier? consumerIdentifier))
            {
                return false;
            }

            return !(await _redisInstance.StreamAddAsync(consumerIdentifier.Stream, "data", JsonSerializer.Serialize(data))).IsNull;
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return false;
        }
    }

    public StreamEntry[] Consume(ConsumerIdentifier consumerIdentifier, RedisValue position, int count, bool noAck)
    {
        try
        {
            return _redisInstance.StreamReadGroup(
                consumerIdentifier.Stream,
                consumerIdentifier.Group,
                consumerIdentifier.Consumer,
                position,
                count,
                noAck
            );
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return Array.Empty<StreamEntry>();
        }
    }

    public bool AckMessage(ConsumerIdentifier consumerIdentifier, RedisValue id)
    {
        try
        {
            return _redisInstance.StreamAcknowledge(consumerIdentifier.Stream, consumerIdentifier.Group, id) > 0;
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return false;
        }
    }

    public async Task<bool> DeleteMessage(ConsumerIdentifier consumerIdentifier, RedisValue[] ids)
    {
        try
        {
            return await _redisInstance.StreamDeleteAsync(consumerIdentifier.Stream, ids) > 0;
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            throw;
        }
    }

    public ConsumerIdentifier? GetConsumerIdentifier(Type type)
    {
        if (!_consumerIdentifiers.ContainsKey(type))
        {
            return null;
        }

        _consumerIdentifiers.TryGetValue(type, out ConsumerIdentifier? consumerIdentifier);

        return consumerIdentifier;
    }
}
