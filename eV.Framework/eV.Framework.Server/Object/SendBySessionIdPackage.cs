// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


namespace eV.Framework.Server.Object;

public class SendBySessionIdPackage
{
    public string SessionId { get; set; } = string.Empty;
    public byte[]? Data { get; set; }
}
