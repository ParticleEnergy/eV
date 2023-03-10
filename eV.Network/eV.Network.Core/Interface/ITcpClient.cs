// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Network.Core.Interface;

public interface ITcpClient : IClient
{
    public event TcpChannelEvent? ConnectCompleted;
    public event TcpChannelEvent? DisconnectCompleted;
}
