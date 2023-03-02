// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Security.Cryptography.X509Certificates;

namespace eV.Network.Tcp.Server;

public class ServerSetting
{
    public int MaxConnectionCount { get; set; } = 1024;
    public int ReceiveBufferSize { get; set; } = 2048;
    public bool TcpKeepAlive { get; set; } = true;

    #region Socket

    public string Host { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 8888;
    public int Backlog { get; set; } = 512;

    #endregion

    #region tls

    public string? TlsTargetHost { get; set; } = "eV";
    public X509CertificateCollection? TlsX509CertificateCollection { get; set; }
    public bool TlsCheckCertificateRevocation { get; set; }

    #endregion
}
