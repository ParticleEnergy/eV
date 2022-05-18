// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

namespace eV.Network.Udp;

public class ServerSetting
{
    public int ReceiveBufferSize { get; set; } = DefaultSetting.ReceiveBufferSize;

    #region Socket
    public string Host { get; set; } = DefaultSetting.Host;
    public int Port { get; set; } = DefaultSetting.Port;
    public string MultiCastHost { get; set; } = DefaultSetting.MultiCastHost;
    public int MultiCastPort { get; set; } = DefaultSetting.MultiCastPort;

    public int MulticastTimeToLive { get; set; } = DefaultSetting.MulticastTimeToLive;
    #endregion
}
