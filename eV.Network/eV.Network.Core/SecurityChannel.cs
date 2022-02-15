// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Sockets;
using eV.EasyLog;
using eV.Network.Core.Interface;
namespace eV.Network.Core;

public class SecurityChannel : IChannel
{
    // private SslStream _sslStream;
    // public SecurityChannel(int receiveBufferSize)
    // {
    //
    // }

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
    public Action<byte[]?>? Receive
    {
        get;
        set;
    }
    public bool Send(byte[] data)
    {
        throw new NotImplementedException();
    }
    public void Open(Socket socket)
    {
        Logger.Error("This method is not supported");
    }
    public void Open(TcpClient tcpClient)
    {

    }
    public void Close()
    {
        throw new NotImplementedException();
    }
}
