// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using eV.Module.Cluster.Communication;
using eV.Module.Cluster.Interface;
using eV.Module.EasyLog;
using StackExchange.Redis;

namespace eV.Module.Cluster;

public class CommunicationManager
{
    public string NodeId { get; }
    public ISessionRegistrationAuthority SessionRegistrationAuthority { get; }

    private readonly Dictionary<Type, ConsumerIdentifier> _consumerIdentifiers;
    private readonly Dictionary<int, ConsumerIdentifier> _sendConsumerIdentifiers;
    private readonly Dictionary<int, ConsumerIdentifier> _sendBroadcastConsumerIdentifiers;

    private readonly ConnectionMultiplexer _redis;

    public static CommunicationManager? Instance { get; private set; }

    private CommunicationManager
    (
        string nodeId,
        ConnectionMultiplexer redis,
        ISessionRegistrationAuthority sessionRegistrationAuthority,
        Dictionary<Type, ConsumerIdentifier> consumerIdentifiers,
        Dictionary<int, ConsumerIdentifier> sendConsumerIdentifiers,
        Dictionary<int, ConsumerIdentifier> sendBroadcastConsumerIdentifiers
    )
    {
        NodeId = nodeId;
        _redis = redis;
        SessionRegistrationAuthority = sessionRegistrationAuthority;
        _consumerIdentifiers = consumerIdentifiers;
        _sendConsumerIdentifiers = sendConsumerIdentifiers;
        _sendBroadcastConsumerIdentifiers = sendBroadcastConsumerIdentifiers;
    }

    public static void InitCommunicationManager
    (
        string nodeId,
        ConnectionMultiplexer redis,
        ISessionRegistrationAuthority sessionRegistrationAuthority,
        Dictionary<Type, ConsumerIdentifier> consumerIdentifiers,
        Dictionary<int, ConsumerIdentifier> sendConsumerIdentifiers,
        Dictionary<int, ConsumerIdentifier> sendBroadcastConsumerIdentifiers
    )
    {
        if (Instance != null)
            return;

        Instance = new CommunicationManager(
            nodeId,
            redis,
            sessionRegistrationAuthority,
            consumerIdentifiers,
            sendConsumerIdentifiers,
            sendBroadcastConsumerIdentifiers
        );
    }

    public async Task<bool> Produce<TValue>(string nodeId, TValue data)
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

            return !(await _redis.GetDatabase().StreamAddAsync(consumerIdentifier.GetStream(nodeId), "data", JsonSerializer.Serialize(data))).IsNull;
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return false;
        }
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

            foreach (string nodeId in await SessionRegistrationAuthority.GetAllNodeIds())
            {
                await _redis.GetDatabase().StreamAddAsync(consumerIdentifier.GetStream(nodeId), "data", JsonSerializer.Serialize(data));
            }

            return true;
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return false;
        }
    }

    public async Task<bool> Send(string sessionId, byte[] body)
    {
        try
        {
            string nodeId = await SessionRegistrationAuthority.GetNodeId(sessionId);
            if (nodeId == string.Empty)
                return false;

            Random random = new();
            return !(
                await _redis.GetDatabase().StreamAddAsync
                (
                    _sendConsumerIdentifiers[random.Next(0, _sendConsumerIdentifiers.Count)].GetStream(nodeId),
                    new[] { new NameValueEntry(CommunicationStream.GetSessionIdKey(), sessionId), new NameValueEntry(CommunicationStream.GetBodyKey(), Convert.ToBase64String(body)) }
                )
            ).IsNull;
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return false;
        }
    }

    public async Task<bool> SendBroadcast(byte[] data)
    {
        try
        {
            foreach (string nodeId in await SessionRegistrationAuthority.GetAllNodeIds())
            {
                if (nodeId.Equals(NodeId))
                    continue;

                Random random = new();
                await _redis.GetDatabase().StreamAddAsync
                (
                    _sendBroadcastConsumerIdentifiers[random.Next(0, _sendBroadcastConsumerIdentifiers.Count)].GetStream(nodeId),
                    "data",
                    Convert.ToBase64String(data)
                    );
            }
            return true;
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return false;
        }
    }

    public StreamEntry[] Consume(string stream, string group, string consumer)
    {
        try
        {
            return _redis.GetDatabase().StreamReadGroup(
                stream,
                group,
                consumer,
                ">",
                1,
                true
            );
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return Array.Empty<StreamEntry>();
        }
    }

    public async Task DeleteMessage(string stream, RedisValue[] ids)
    {
        await _redis.GetDatabase().StreamDeleteAsync(stream, ids);
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
