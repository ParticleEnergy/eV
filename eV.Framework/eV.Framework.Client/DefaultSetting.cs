// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Net.Sockets;
using System.Security.Authentication;
namespace eV.Framework.Client;

internal static class DefaultSetting
{
    public const string Logo = @"
      _    __   _________            __
  ___| |  / /  / ____/ (_)__  ____  / /_
 / _ \ | / /  / /   / / / _ \/ __ \/ __/
/  __/ |/ /  / /___/ / /  __/ / / / /_
\___/|___/   \____/_/_/\___/_/ /_/\__/
";
    public const string LoggerName = "eV.Clinet";

    public const string Host = "127.0.0.1";
    public const int Port = 8888;
    public const SocketType SocketType = (SocketType)1;
    public const ProtocolType ProtocolType = (ProtocolType)6;
    public const int ReceiveBufferSize = 2048;
    public const int TcpKeepAliveTime = 60;
    public const int TcpKeepAliveInterval = 3;
    public const int TcpKeepAliveRetryCount = 3;

    public const SslProtocols SslProtocols = (SslProtocols)192;
    public const string TargetHost = "eV";
}
