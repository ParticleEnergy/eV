// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Security.Authentication;
namespace eV.Framework.Unity;

public class UnitySetting
{
    public string Host { get; set; } = DefaultSetting.Host;
    public int Port { get; set; } = DefaultSetting.Port;
    public int ReceiveBufferSize { get; set; } = DefaultSetting.ReceiveBufferSize;
    public string HandlerNamespace { get; set; } = string.Empty;
    public string DataStructNamespace { get; set; } = string.Empty;
    public string GameProfilePath { get; set; } = string.Empty;

    public SslProtocols SslProtocols { get; set; } = DefaultSetting.SslProtocols;

    public string TargetHost { get; set; } = DefaultSetting.TargetHost;

    public string CertFile { get; set; } = string.Empty;
    public int TcpKeepAliveInterval { get; set; } = DefaultSetting.TcpKeepAliveInterval;
}
