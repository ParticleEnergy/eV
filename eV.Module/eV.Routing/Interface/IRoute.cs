// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

namespace eV.Routing.Interface;

public interface IRoute
{
    public IHandler Handler { get; }
    public Type ContentType { get; }
}
