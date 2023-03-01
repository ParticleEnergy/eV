// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Net.Sockets;

namespace eV.Network.Core.Interface;

public interface ITcpChannel : IChannel
{
    public event TcpChannelEvent? OpenCompleted;
    public event TcpChannelEvent? CloseCompleted;
    public void Open(Socket socket);
    public Action<byte[]?>? Receive { set; }
    public bool Send(byte[] data);
}
