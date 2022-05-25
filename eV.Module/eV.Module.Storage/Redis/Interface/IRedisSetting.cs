// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

namespace eV.Module.Storage.Redis.Interface;

public interface IRedisSetting
{
    public List<IAddress> Address { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }
    public int? Database { get; set; }
}