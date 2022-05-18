// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

namespace eV.Network.Core.Interface;

public interface ITcpChannel : IChannel
{
    public event TcpChannelEvent? OpenCompleted;
    public event TcpChannelEvent? CloseCompleted;
    public bool Send(byte[] data);
}
