// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Net.Sockets;
using System.Security.Authentication;
namespace eV.Network.Security.Client;

internal static class DefaultSetting
{
    public const string Address = "127.0.0.1";
    public const int Port = 8888;
    public const SocketType SocketType = (SocketType)1;
    public const ProtocolType ProtocolType = (ProtocolType)6;
    public const int ReceiveBufferSize = 2048;
    public const SslProtocols SslProtocols = (SslProtocols)192;
    public const string TargetHost = "eV";
    public const string CertFile = "";
#if !NETSTANDARD
    public const int TcpKeepAliveTime = 60;
    public const int TcpKeepAliveInterval = 3;
    public const int TcpKeepAliveRetryCount = 3;
#endif
}
