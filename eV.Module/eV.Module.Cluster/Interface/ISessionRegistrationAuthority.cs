// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Module.Cluster.Interface;

public interface ISessionRegistrationAuthority
{
    public void Registry(string pod, string sessionId);
    public void Deregister(string pod, string sessionId);
}
