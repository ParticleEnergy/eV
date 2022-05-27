// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Framework.Server.Options;

public class ServerOptions
{
    public const string Keyword = "Server";

    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public int Backlog { get; set; }
    public int MaxConnectionCount { get; set; }
    public int ReceiveBufferSize { get; set; }
    public int TcpKeepAliveTime { get; set; }
    public int TcpKeepAliveInterval { get; set; }
    public int TcpKeepAliveRetryCount { get; set; }
    public int SessionMaximumIdleTime { get; set; }
}
