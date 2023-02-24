// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Framework.Server.Options;

public class ClusterOption
{
    public string ClusterName { get; set; } = string.Empty;
    public int ConsumeSendPipelineNumber { get; set; }
    public int ConsumeSendGroupPipelineNumber { get; set; }
    public int ConsumeSendBroadcastPipelineNumber { get; set; }
    public RedisOption Redis { get; set; } = new();
}
