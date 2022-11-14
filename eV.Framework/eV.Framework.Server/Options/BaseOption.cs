// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Framework.Server.Options;

public class BaseOption
{
    public bool Debug { get; set; }
    public bool Compress { get; set; }
    public string ProjectAssemblyString { get; set; } = string.Empty;
    public string PublicObjectAssemblyString { get; set; } = string.Empty;
    public string GameProfilePath { get; set; } = string.Empty;
    public bool GameProfileMonitoringChange { get; set; }
}
