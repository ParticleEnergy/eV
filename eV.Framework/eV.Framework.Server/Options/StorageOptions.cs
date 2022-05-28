// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Storage.Redis.Config;
namespace eV.Framework.Server.Options;

public class StorageOptions
{
    public const string Keyword = "Storage";
    public Dictionary<string, string>? Mongodb { get; set; }
    public Dictionary<string, RedisOptions>? Redis { get; set; }
}
