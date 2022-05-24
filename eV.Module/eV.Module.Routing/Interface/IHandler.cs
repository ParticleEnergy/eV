// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

namespace eV.Module.Routing.Interface;

public interface IHandler
{
    public void Run(ISession session, object message);
}
