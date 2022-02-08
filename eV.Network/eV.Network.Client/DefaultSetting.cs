// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Net.Sockets;
namespace eV.Network.Client
{
    internal static class DefaultSetting
    {
        public const string Address = "127.0.0.1";
        public const int Port = 8888;
        public const SocketType SocketType = (SocketType)1;
        public const ProtocolType ProtocolType = (ProtocolType)6;
        public const int ReceiveBufferSize = 2048;
        public const string LoggerName = "eV.Network.Client";
    }
}
