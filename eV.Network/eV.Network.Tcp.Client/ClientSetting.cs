// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Security.Cryptography.X509Certificates;

namespace eV.Network.Tcp.Client;

public class ClientSetting
{
    public int ReceiveBufferSize { get; set; } = DefaultSetting.ReceiveBufferSize;

    #region Socket

    public string Host { get; set; } = DefaultSetting.Host;
    public int Port { get; set; } = DefaultSetting.Port;

    #endregion

    public bool TcpKeepAlive { get; set; } = DefaultSetting.TcpKeepAlive;

    #region tls

    public string? TlsTargetHost { get; set; } = DefaultSetting.TlsTargetHost;
    public X509CertificateCollection? TlsX509CertificateCollection { get; set; } = DefaultSetting.TlsX509CertificateCollection;
    public bool TlsCheckCertificateRevocation { get; set; } = DefaultSetting.TlsCheckCertificateRevocation;

    #endregion
}
