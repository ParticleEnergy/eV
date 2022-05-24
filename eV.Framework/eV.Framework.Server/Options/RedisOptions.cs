// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

namespace eV.Framework.Server.Options;

public record RedisOptions
{
    public RedisOptions()
    {

    }
    public RedisOptions(List<AddressOptions>? address, string? user, string? password, int? database)
    {
        Address = address;
        User = user;
        Password = password;
        Database = database;
    }
    public List<AddressOptions>? Address { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }
    public int? Database { get; set; }
}
