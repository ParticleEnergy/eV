// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.


using System.Collections.Concurrent;
namespace eV.Network.Core;

public class ChannelManager
{
    private readonly ConcurrentDictionary<string, Channel> _channels;

    public ChannelManager()
    {
        _channels = new ConcurrentDictionary<string, Channel>();
    }

    public Channel? GetChannel(string channelId)
    {
        return _channels.TryGetValue(channelId, out Channel? result) ? result : null;
    }

    public ConcurrentDictionary<string, Channel> GetAllChannel()
    {
        return _channels;
    }

    public int GetCount()
    {
        return _channels.Count;
    }

    public bool Add(Channel channel)
    {
        _channels[channel.ChannelId] = channel;
        return true;
    }

    public bool Remove(Channel channel)
    {
        return _channels.TryRemove(channel.ChannelId, out Channel? _);
    }
}
