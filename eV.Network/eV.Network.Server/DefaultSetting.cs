// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Net.Sockets;
namespace eV.Network.Server
{
    internal static class DefaultSetting
    {
        public const string Address = "0.0.0.0";
        public const int Port = 8888;
        public const SocketType SocketType = (SocketType)1;
        public const ProtocolType ProtocolType = (ProtocolType)6;
        public const int Backlog = 512;
        public const int MaxConnectionCount = 1024;
        public const int ReceiveBufferSize = 2048;
        public const int TcpKeepAliveTime = 60;
        public const int TcpKeepAliveInterval = 3;
        public const int TcpKeepAliveRetryCount = 3;
    }
}
