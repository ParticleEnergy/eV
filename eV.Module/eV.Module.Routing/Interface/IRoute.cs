// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Module.Routing.Interface;

public interface IRoute
{
    public IHandler Handler { get; }
    public Type ContentType { get; }
}
