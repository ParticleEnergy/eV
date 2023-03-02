// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.EasyLog.Interface;

namespace eV.Framework.Unity;

public class UnitySetting
{
    public string Host { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 8888;
    public int ReceiveBufferSize { get; set; } = 2048;
    public string ProjectAssemblyString { get; set; } = string.Empty;
    public string PublicObjectAssemblyString { get; set; } = string.Empty;

    public string? TlsTargetHost { get; set; } = "eV";
    public byte[]? TlsCertData { get; set; } = null;
    public string? TlsCertPassword { get; set; } = null;
    public bool TlsCheckCertificateRevocation { get; set; } = false;
    public bool TcpKeepAlive { get; set; } = true;
    public ILog? Log { get; set; } = null;
}
