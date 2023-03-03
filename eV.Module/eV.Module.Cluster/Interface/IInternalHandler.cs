// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Module.Cluster.Interface;

public interface IInternalHandler
{
    public bool IsMultipleSubscribers { get; set; }

    public void Run();
}
