// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
namespace eV.Server.Options
{
    public class StorageOptions
    {
        public const string Keyword = "Storage";
        public Dictionary<string, string>? Mongodb { get; set; }
        public Dictionary<string, RedisOptions>? Redis { get; set; }
    }
}
