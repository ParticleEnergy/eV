// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


namespace eV.Module.Cluster;

public class ChannelIdentifier
{
    private readonly string _clusterId;
    private readonly string _channel;
    public bool IsMultipleSubscribers { get; }

    public ChannelIdentifier(string clusterId, string channel, bool isMultipleSubscribers)
    {
        _clusterId = clusterId;
        _channel = channel;
        IsMultipleSubscribers = isMultipleSubscribers;
    }

    public string GetChannel(string nodeId)
    {
        return $"eV:Cluster:{_clusterId}:Channel:{_channel}:Node:{nodeId}";
    }

    public string GetChannel()
    {
        return $"eV:Cluster:{_clusterId}:Channel:{_channel}";
    }
}
