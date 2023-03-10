// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Net;

namespace eV.Network.Core.Interface;

public interface IChannel
{
    public DateTime? ConnectedDateTime { get; }
    public DateTime? LastReceiveDateTime { get; }
    public DateTime? LastSendDateTime { get; }
    public RunState ChannelState { get; }
    public string ChannelId { get; }
    public EndPoint? RemoteEndPoint { get; }
    public void Close();
}
