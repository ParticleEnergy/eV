// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.Module.Storage.Redis.Interface;
namespace eV.Framework.Server.Options;

public record RedisOptions : IRedisSetting
{
    public RedisOptions(List<IAddress> address)
    {
        Address = address;
    }
    public RedisOptions(List<IAddress> address, string? user, string? password, int? database)
    {
        Address = address;
        User = user;
        Password = password;
        Database = database;
    }
    public List<IAddress> Address { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }
    public int? Database { get; set; }
}
