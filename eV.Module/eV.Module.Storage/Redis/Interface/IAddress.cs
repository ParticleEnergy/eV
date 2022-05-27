// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Module.Storage.Redis.Interface;

public interface IAddress
{
    public string Host { get; set; }
    public int Port { get; set; }
}
