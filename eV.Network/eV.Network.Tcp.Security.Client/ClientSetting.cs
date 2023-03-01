// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Security.Authentication;

namespace eV.Network.Tcp.Security.Client;

public class ClientSetting
{
    public int ReceiveBufferSize { get; set; } = DefaultSetting.ReceiveBufferSize;

    #region Socket

    public string Host { get; set; } = DefaultSetting.Host;
    public int Port { get; set; } = DefaultSetting.Port;
    public string TargetHost { get; set; } = DefaultSetting.TargetHost;
    public string CertFile { get; set; } = string.Empty;
    public SslProtocols SslProtocols { get; set; } = DefaultSetting.SslProtocols;

    #endregion

    public bool TcpKeepAlive { get; set; } = DefaultSetting.TcpKeepAlive;
    public int TcpKeepAliveTime { get; set; } = DefaultSetting.TcpKeepAliveTime;
    public int TcpKeepAliveInterval { get; set; } = DefaultSetting.TcpKeepAliveInterval;
}
