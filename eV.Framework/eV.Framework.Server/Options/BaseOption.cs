// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Framework.Server.Options;

public class BaseOption
{
    public string HandlerNamespace { get; set; } = string.Empty;
    public string PublicObjectNamespace { get; set; } = string.Empty;
    public string GameProfilePath { get; set; } = string.Empty;
    public bool GameProfileMonitoringChange { get; set; }
}
