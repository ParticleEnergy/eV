// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Module.Cluster.Interface;

public interface ISessionRegistrationAuthority
{
    public void Registry(string sessionId);
    public void Deregister(string sessionId);
    public Task<List<string>> GetAllNodeIds();
    public Task<string> GetNodeId(string sessionId);
    public Task<List<string>> GetAllSessionIds();
    public Task<List<string>> GetSessionIdsByNode(string nodeId);
}
