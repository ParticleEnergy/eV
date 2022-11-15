// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Network.Udp;

public class ServiceSetting
{
    public int ReceiveBufferSize { get; set; } = DefaultSetting.ReceiveBufferSize;
    public int MaxConcurrentSend { get; set; } = DefaultSetting.MaxConcurrentSend;

    #region Socket

    public int ListenPort { get; set; } = DefaultSetting.ListenPort;
    public string Localhost { get; set; } = DefaultSetting.Localhost;
    public string MultiCastHost { get; set; } = DefaultSetting.MultiCastHost;
    public int MulticastTimeToLive { get; set; } = DefaultSetting.MulticastTimeToLive;
    public bool MulticastLoopback { get; set; } = DefaultSetting.MulticastLoopback;

    #endregion

    public int CommPort { get; set; } = DefaultSetting.CommPort;
}
