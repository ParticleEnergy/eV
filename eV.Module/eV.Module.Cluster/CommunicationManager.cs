// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Text.Json;
using eV.Module.Cluster.Interface;
using eV.Module.EasyLog;
using StackExchange.Redis;

namespace eV.Module.Cluster;

public class CommunicationManager
{
    public static CommunicationManager? Instance { get; private set; }
    public string NodeId { get; }
    public ISessionRegistrationAuthority SessionRegistrationAuthority { get; }

    private readonly ISubscriber _subscriber;
    private readonly Dictionary<Type, ChannelIdentifier> _channelIdentifiers;


    private CommunicationManager(
        string nodeId,
        IConnectionMultiplexer redis,
        ISessionRegistrationAuthority sessionRegistrationAuthority,
        Dictionary<Type, ChannelIdentifier> channelIdentifiers
    )
    {
        NodeId = nodeId;
        _subscriber = redis.GetSubscriber();
        SessionRegistrationAuthority = sessionRegistrationAuthority;
        _channelIdentifiers = channelIdentifiers;
    }

    public static void InitCommunicationManager
    (
        string nodeId,
        ConnectionMultiplexer redis,
        ISessionRegistrationAuthority sessionRegistrationAuthority,
        Dictionary<Type, ChannelIdentifier> channelIdentifiers
    )
    {
        if (Instance != null)
            return;

        Instance = new CommunicationManager(
            nodeId,
            redis,
            sessionRegistrationAuthority,
            channelIdentifiers
        );
    }

    public async Task<bool> SendInternalMessage<T>(string nodeId, T data)
    {
        try
        {
            var type = typeof(T);
            if (!_channelIdentifiers.ContainsKey(type))
            {
                return false;
            }

            if (!_channelIdentifiers.TryGetValue(type, out ChannelIdentifier? channelIdentifier))
            {
                return false;
            }
            return await _subscriber.PublishAsync(channelIdentifier.GetChannel(nodeId), JsonSerializer.Serialize(data)) > 0;
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return false;
        }
    }

    public async Task<bool> SendInternalMessage<T>(T data)
    {
        try
        {
            var type = typeof(T);
            if (!_channelIdentifiers.ContainsKey(type))
            {
                return false;
            }

            if (!_channelIdentifiers.TryGetValue(type, out ChannelIdentifier? channelIdentifier))
            {
                return false;
            }

            if (channelIdentifier.IsMultipleSubscribers)
            {
                await _subscriber.PublishAsync(channelIdentifier.GetChannel(), JsonSerializer.Serialize(data));
            }
            else
            {
                foreach (string nodeId in await SessionRegistrationAuthority.GetAllNodeIds())
                {
                    await _subscriber.PublishAsync(channelIdentifier.GetChannel(nodeId), JsonSerializer.Serialize(data));
                }
            }

            return true;
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return false;
        }
    }

    public async Task Subscribe(Type type, Action<RedisChannel, RedisValue> handler)
    {
        try
        {
            if (!_channelIdentifiers.ContainsKey(type))
            {
                return;
            }

            if (!_channelIdentifiers.TryGetValue(type, out ChannelIdentifier? channelIdentifier))
            {
                return;
            }

            if (channelIdentifier.IsMultipleSubscribers)
            {
                await _subscriber.SubscribeAsync(channelIdentifier.GetChannel(), handler);
                Logger.Info($"Cluster channel [{channelIdentifier.GetChannel()}] Handler [{type.Name}] subscribe succeeded");
            }
            else
            {
                await _subscriber.SubscribeAsync(channelIdentifier.GetChannel(NodeId), handler);
                Logger.Info($"Cluster channel [{channelIdentifier.GetChannel(NodeId)}] Handler [{type.Name}] subscribe succeeded");
            }
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }
}
