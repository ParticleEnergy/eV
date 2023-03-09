// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Framework.Server.Options;

public class ServerOption
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public int Backlog { get; set; }
    public int MaxConnectionCount { get; set; }
    public int ReceiveBufferSize { get; set; }
    public bool TcpKeepAlive { get; set; } = true;
    public int SessionMaximumIdleTime { get; set; }

    public string TlsTargetHost { get; set; } = string.Empty;
    public string TlsCertFile { get; set; } = string.Empty;
    public string? TlsCertPassword { get; set; }
    public bool TlsCheckCertificateRevocation { get; set; }
}
