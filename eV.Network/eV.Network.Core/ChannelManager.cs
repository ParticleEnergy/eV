// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using System.Collections.Concurrent;
using eV.Network.Core.Interface;

namespace eV.Network.Core;

public class ChannelManager
{
    private readonly ConcurrentDictionary<string, IChannel> _channels;

    public ChannelManager()
    {
        _channels = new ConcurrentDictionary<string, IChannel>();
    }

    public IChannel? GetChannel(string channelId)
    {
        return _channels.TryGetValue(channelId, out IChannel? result) ? result : null;
    }

    public ConcurrentDictionary<string, IChannel> GetAllChannel()
    {
        return _channels;
    }

    public int GetCount()
    {
        return _channels.Count;
    }

    public bool Add(IChannel channel)
    {
        _channels[channel.ChannelId] = channel;
        return true;
    }

    public bool Remove(IChannel channel)
    {
        return _channels.TryRemove(channel.ChannelId, out IChannel? _);
    }
}
