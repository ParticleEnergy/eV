// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

namespace eV.Network.Core.Interface;

public interface IClient
{
    public event ChannelEvent? ConnectCompleted;
    public event ChannelEvent? DisconnectCompleted;
    public void Connect();
    public void Disconnect();
}
