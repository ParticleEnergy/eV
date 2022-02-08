// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

namespace eV.Server.Options
{
    public class BaseOptions
    {
        public const string Keyword = "Base";

        public string ProjectNamespace { get; set; } = string.Empty;
        public string GameProfilePath { get; set; } = string.Empty;
        public bool GameProfileMonitoringChange { get; set; }
    }
}
