// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Framework.Server.Options;

public class RedisOption
{
    public string[] Address { get; set; } = Array.Empty<string>();
    public string? User { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;
    public int? Database { get; set; } = 1;
    public int? Keepalive { get; set; } = 60;
}
