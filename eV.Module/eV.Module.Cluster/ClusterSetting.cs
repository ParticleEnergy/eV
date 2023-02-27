// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.EasyLog;

namespace eV.Module.Cluster;

public class CommunicationSetting
{
    public int SendBatchProcessingQuantity { get; set; } = 1;
    public int SendBroadcastBatchProcessingQuantity { get; set; } = 1;

    public Func<string, byte[], bool> SendAction { get; set; } = delegate
    {
        Logger.Error("SendAction not defined");
        return false;
    };

    public Action<byte[]> SendBroadcastAction { get; set; } = delegate { Logger.Error("SendBroadcastAction not defined"); };
}
