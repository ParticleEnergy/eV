// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Network.Tcp.Server;

internal static class DefaultSetting
{
    public const string Host = "0.0.0.0";
    public const int Port = 8888;
    public const int Backlog = 512;
    public const int MaxConnectionCount = 1024;
    public const int ReceiveBufferSize = 2048;
    public const bool TcpKeepAlive = true;
    public const int TcpKeepAliveTime = 600;
    public const int TcpKeepAliveInterval = 60;
}
