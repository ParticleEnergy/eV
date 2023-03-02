// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Security.Cryptography.X509Certificates;

namespace eV.Network.Tcp.Client;

internal static class DefaultSetting
{
    public const string Host = "127.0.0.1";
    public const int Port = 8888;
    public const int ReceiveBufferSize = 2048;

    public const bool TcpKeepAlive = true;

    public const string TlsTargetHost = "eV";
    public const X509CertificateCollection? TlsX509CertificateCollection = null;
    public const bool TlsCheckCertificateRevocation = false;
}
