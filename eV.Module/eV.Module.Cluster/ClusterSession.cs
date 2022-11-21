// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Cluster.Interface;

namespace eV.Module.Cluster;

public class ClusterSession
{
    private readonly ICommunicationQueue _communicationQueue;
    private readonly ISessionRegistrationAuthority _sessionRegistrationAuthority;

    public ClusterSession(ISessionRegistrationAuthority sessionRegistrationAuthority,
        ICommunicationQueue communicationQueue)
    {
        _sessionRegistrationAuthority = sessionRegistrationAuthority;
        _communicationQueue = communicationQueue;
    }

    public void Send(string sessionId, byte[] data)
    {
        _communicationQueue.Send(sessionId, data);
    }

    public void SendGroup(string groupId, byte[] data)
    {
        _communicationQueue.SendGroup(groupId, data);
    }

    public void SendBroadcast(byte[] data)
    {
        _communicationQueue.SendBroadcast(data);
    }

    public void Registry(string sessionId)
    {
        _sessionRegistrationAuthority.Registry(sessionId);
    }

    public void Deregister(string sessionId)
    {
        _sessionRegistrationAuthority.Deregister(sessionId);
    }

    public void CreateGroup(string groupId)
    {

    }

    public void DeleteGroup(string groupId)
    {

    }
}
