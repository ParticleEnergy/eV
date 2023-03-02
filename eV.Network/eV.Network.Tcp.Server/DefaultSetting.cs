// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Security.Cryptography.X509Certificates;

namespace eV.Network.Tcp.Server;

internal static class DefaultSetting
{
    public const string Host = "0.0.0.0";
    public const int Port = 8888;
    public const int Backlog = 512;
    public const int MaxConnectionCount = 1024;
    public const int ReceiveBufferSize = 2048;
    public const bool TcpKeepAlive = true;

    public const string TlsTargetHost = "127.0.0.1";
    public const X509CertificateCollection? TlsX509CertificateCollection = null;
    public const bool TlsCheckCertificateRevocation = false;
}
