// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

namespace eV.Unity;

public class UnitySetting
{
    public string Host { get; set; } = DefaultSetting.Host;
    public int Port { get; set; } = DefaultSetting.Port;
    public int ReceiveBufferSize { get; set; } = DefaultSetting.ReceiveBufferSize;
    public string ProjectNamespace { get; set; } = string.Empty;
    public string GameProfilePath { get; set; } = string.Empty;

    public int TcpKeepAliveInterval { get; set; } = DefaultSetting.TcpKeepAliveInterval;
}
