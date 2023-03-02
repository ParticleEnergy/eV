// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.EasyLog.Interface;

namespace eV.Framework.Unity;

public class UnitySetting
{
    public string Host { get; set; } = DefaultSetting.Host;
    public int Port { get; set; } = DefaultSetting.Port;
    public int ReceiveBufferSize { get; set; } = DefaultSetting.ReceiveBufferSize;
    public string ProjectAssemblyString { get; set; } = string.Empty;
    public string PublicObjectAssemblyString { get; set; } = string.Empty;

    public string? TlsTargetHost { get; set; } = DefaultSetting.TlsTargetHost;
    public byte[]? TlsCertData { get; set; } = null;
    public string? TlsCertPassword { get; set; } = null;
    public bool TlsCheckCertificateRevocation { get; set; } = false;
    public bool TcpKeepAlive { get; set; } = DefaultSetting.TcpKeepAlive;
    public ILog? Log { get; set; } = null;
}
