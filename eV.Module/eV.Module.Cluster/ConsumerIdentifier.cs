// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


namespace eV.Module.Cluster;

public class ConsumerIdentifier
{
    private readonly string _clusterId;
    private readonly string _stream;

    public ConsumerIdentifier(string clusterId, string stream)
    {
        _clusterId = clusterId;
        _stream = stream;
    }

    public string GetStream(string nodeId)
    {
        return $"eV:Cluster:{_clusterId}:Node:{nodeId}:Stream:{_stream}";
    }

    public string GetGroup(string nodeId)
    {
        return $"eV:Cluster:{_clusterId}:Node:{nodeId}:Group:{_stream}";
    }

    public string GetConsumer(string nodeId)
    {
        return $"eV:Cluster:{_clusterId}:Node:{nodeId}:Consumer:{_stream}";
    }
}
