// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using System.Text.Json;
using eV.Module.EasyLog;
using StackExchange.Redis;

namespace eV.Module.Queue;

public class MessageProcessor
{
    private readonly ConnectionMultiplexer _redis;
    private readonly Dictionary<Type, ConsumerIdentifier> _consumerIdentifiers;

    public static MessageProcessor? Instance { get; private set; }

    private MessageProcessor(ConnectionMultiplexer redis, Dictionary<Type, ConsumerIdentifier> consumerIdentifiers)
    {
        _redis = redis;
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

            return !(await _redis.GetDatabase().StreamAddAsync(consumerIdentifier.Stream, "data", JsonSerializer.Serialize(data))).IsNull;
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
            return _redis.GetDatabase().StreamReadGroup(
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
        return _redis.GetDatabase().StreamAcknowledge(consumerIdentifier.Stream, consumerIdentifier.Group, id) > 0;
    }

    public async Task DeleteMessage(ConsumerIdentifier consumerIdentifier, RedisValue[] ids)
    {
        await _redis.GetDatabase().StreamDeleteAsync(consumerIdentifier.Stream, ids);
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
