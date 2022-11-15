// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Network.Tcp.Client;

public class ClientSetting
{
    public int ReceiveBufferSize { get; set; } = DefaultSetting.ReceiveBufferSize;

    #region Socket

    public string Host { get; set; } = DefaultSetting.Host;
    public int Port { get; set; } = DefaultSetting.Port;

    #endregion

#if !NETSTANDARD
    public int TcpKeepAliveTime { get; set; } = DefaultSetting.TcpKeepAliveTime;
    public int TcpKeepAliveInterval { get; set; } = DefaultSetting.TcpKeepAliveInterval;
    public int TcpKeepAliveRetryCount { get; set; } = DefaultSetting.TcpKeepAliveRetryCount;
#endif
}
