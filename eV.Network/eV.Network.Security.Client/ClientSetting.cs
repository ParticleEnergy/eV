// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Security.Authentication;
namespace eV.Network.Security.Client;

public class ClientSetting
{
    public int ReceiveBufferSize { get; set; } = DefaultSetting.ReceiveBufferSize;
    #region Socket
    public string Address { get; set; } = DefaultSetting.Address;
    public int Port { get; set; } = DefaultSetting.Port;
    public string TargetHost { get; set; } = DefaultSetting.TargetHost;
    public string CertFile { get; set; } = string.Empty;
    public SslProtocols SslProtocols { get; set; } = DefaultSetting.SslProtocols;
    #endregion

#if !NETSTANDARD
    public int TcpKeepAliveTime { get; set; } = DefaultSetting.TcpKeepAliveTime;
    public int TcpKeepAliveInterval { get; set; } = DefaultSetting.TcpKeepAliveInterval;
    public int TcpKeepAliveRetryCount { get; set; } = DefaultSetting.TcpKeepAliveRetryCount;
#endif
}
