// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Network.Tcp.Server;

public class ServerSetting
{
    public int MaxConnectionCount { get; set; } = DefaultSetting.MaxConnectionCount;
    public int ReceiveBufferSize { get; set; } = DefaultSetting.ReceiveBufferSize;
    public bool TcpKeepAlive { get; set; } = DefaultSetting.TcpKeepAlive;

    #region Socket

    public string Host { get; set; } = DefaultSetting.Host;
    public int Port { get; set; } = DefaultSetting.Port;
    public int Backlog { get; set; } = DefaultSetting.Backlog;

    #endregion
}
