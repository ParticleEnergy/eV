// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

namespace eV.Module.Cluster.Interface;

public interface ISessionStorage
{
    public bool Del(string sessionId);
    public bool Add(string sessionId);
    public bool Get(string sessionId);
    public bool Edit(string sessionId);
}

