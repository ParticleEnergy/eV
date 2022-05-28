// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Module.Storage.Redis.Config;

public class RedisOptions
{
    public List<RedisAddress> Address
    {
        get;
        set;
    } = new();
    public string? User
    {
        get;
        set;
    }
    public string? Password
    {
        get;
        set;
    }
    public int? Database
    {
        get;
        set;
    }
}
