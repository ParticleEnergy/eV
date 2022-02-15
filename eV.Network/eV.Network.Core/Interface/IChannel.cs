// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Net;
namespace eV.Network.Core.Interface;

public interface IChannel
{
    public event ChannelEvent? CloseCompleted;
    public DateTime? ConnectedDateTime
    {
        get;
    }
    public DateTime? LastReceiveDateTime
    {
        get;
    }
    public DateTime? LastSendDateTime
    {
        get;
    }
    public RunState ChannelState
    {
        get;
    }
    public string ChannelId
    {
        get;
    }
    public EndPoint? RemoteEndPoint
    {
        get;
    }
    public Action<byte[]?>? Receive { set; }
    public bool Send(byte[] data);
    public void Close();
}
