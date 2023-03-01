// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Security.Authentication;

namespace eV.Network.Tcp.Security.Client;

internal static class DefaultSetting
{
    public const string Host = "127.0.0.1";
    public const int Port = 8888;
    public const int ReceiveBufferSize = 2048;
    public const SslProtocols SslProtocols = (SslProtocols)192;
    public const string TargetHost = "eV";
    public const bool TcpKeepAlive = true;
}
