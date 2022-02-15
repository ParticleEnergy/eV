// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Sockets;
using eV.EasyLog;
namespace eV.Network.Core;

public delegate void ChannelEvent(Channel channel);
public class Channel
{
    public Channel(int receiveBufferSize)
    {
        ChannelId = Guid.NewGuid().ToString();
        ChannelState = RunState.Off;

        // Completed
        SocketAsyncEventArgsCompleted socketAsyncEventArgsCompleted = new();
        socketAsyncEventArgsCompleted.ProcessReceive += ProcessReceive;
        socketAsyncEventArgsCompleted.ProcessSend += ProcessSend;
        socketAsyncEventArgsCompleted.ProcessDisconnect += ProcessDisconnect;
        // Receive
        _receiveBuffer = new byte[receiveBufferSize];
        _receiveSocketAsyncEventArgs = new SocketAsyncEventArgs();
        _receiveSocketAsyncEventArgs.Completed += socketAsyncEventArgsCompleted.OnCompleted;
        _receiveSocketAsyncEventArgs.SetBuffer(_receiveBuffer, 0, receiveBufferSize);
        // Send
        _sendSocketAsyncEventArgs = new SocketAsyncEventArgs();
        _sendSocketAsyncEventArgs.Completed += socketAsyncEventArgsCompleted.OnCompleted;
        // Disconnect
        _disconnectSocketAsyncEventArgs = new SocketAsyncEventArgs();
        _disconnectSocketAsyncEventArgs.Completed += socketAsyncEventArgsCompleted.OnCompleted;
        _disconnectSocketAsyncEventArgs.DisconnectReuseSocket = false;
    }

    #region Error
    private void Error(ChannelError channelError)
    {
        switch (channelError)
        {
            case ChannelError.SocketIsNull:
                Close();
                break;
            case ChannelError.SocketNotConnect:
                Close();
                break;
            case ChannelError.SocketError:
                Close();
                break;
            case ChannelError.SocketBytesTransferredIsZero:
                Close();
                break;
            default:
                Logger.Error("ChannelError not found");
                break;
        }
    }
    #endregion

    #region Event
    public event ChannelEvent? OpenCompleted;
    public event ChannelEvent? CloseCompleted;
    #endregion

    #region Public
    #region Time
    public DateTime? ConnectedDateTime
    {
        get;
        private set;
    }
    public DateTime? LastReceiveDateTime
    {
        get;
        private set;
    }
    public DateTime? LastSendDateTime
    {
        get;
        private set;
    }
    #endregion
    public RunState ChannelState
    {
        get;
        private set;
    }
    public string ChannelId
    {
        get;
    }
    public EndPoint? RemoteEndPoint
    {
        get;
        private set;
    }
    #endregion

    #region Resource
    private Socket? _socket;
    private readonly SocketAsyncEventArgs _sendSocketAsyncEventArgs;
    private readonly SocketAsyncEventArgs _receiveSocketAsyncEventArgs;
    private readonly SocketAsyncEventArgs _disconnectSocketAsyncEventArgs;
    private readonly byte[] _receiveBuffer;
    #endregion

    #region Operate
    public void Open(Socket socket)
    {
        if (ChannelState == RunState.On)
        {
            Logger.Warn("The channel is already turned on");
            return;
        }
        try
        {
            Init(socket);

            OpenCompleted?.Invoke(this);
            Logger.Info($"Channel {ChannelId} {RemoteEndPoint} open");
            Logger.Info(StartReceive() ? $"Channel {ChannelId} {RemoteEndPoint} start receive" : $"Channel {ChannelId} {RemoteEndPoint} start receive failed");
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }
    public void Close()
    {
        if (ChannelState == RunState.Off)
            return;
        ChannelState = RunState.Off;
        try
        {
            if (_socket is { Connected: true })
            {
                _socket.Shutdown(SocketShutdown.Both);
                if (!_socket.DisconnectAsync(_disconnectSocketAsyncEventArgs))
                    ProcessDisconnect(_disconnectSocketAsyncEventArgs);
            }
            else
            {
                Release();
            }
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }
    /// <summary>
    ///     释放资源
    /// </summary>
    private void Release()
    {
        _socket?.Close();
        _socket = null;
        _receiveSocketAsyncEventArgs.AcceptSocket = null;
        _sendSocketAsyncEventArgs.AcceptSocket = null;
        Array.Clear(_receiveBuffer, 0, _receiveBuffer.Length);

        Logger.Info($"Channel {ChannelId} {RemoteEndPoint} close");
        CloseCompleted?.Invoke(this);
    }
    /// <summary>
    ///     重置Channel
    /// </summary>
    private void Init(Socket socket)
    {
        _socket = socket;
        ChannelState = RunState.On;
        ConnectedDateTime = DateTime.Now;
        RemoteEndPoint = _socket?.RemoteEndPoint;
    }
    #endregion

    #region IO
    private bool StartReceive()
    {
        if (ChannelState == RunState.Off)
            return false;
        if (_socket == null)
        {
            Error(ChannelError.SocketIsNull);
            return false;
        }
        if (!_socket.Connected)
        {
            Error(ChannelError.SocketNotConnect);
            return false;
        }
        if (!_socket.ReceiveAsync(_receiveSocketAsyncEventArgs))
            ProcessReceive(_receiveSocketAsyncEventArgs);
        return true;
    }
    public Action<byte[]?>? Receive;

    public bool Send(byte[] data)
    {
        if (ChannelState == RunState.Off)
            return false;
        if (_socket == null)
        {
            Error(ChannelError.SocketIsNull);
            return false;
        }
        if (!_socket.Connected)
        {
            Error(ChannelError.SocketNotConnect);
            return false;
        }
        lock (_sendSocketAsyncEventArgs)
        {
            _sendSocketAsyncEventArgs.SetBuffer(data, 0, data.Length);
            if (!_socket!.SendAsync(_sendSocketAsyncEventArgs))
                ProcessSend(_sendSocketAsyncEventArgs);
        }
        return true;
    }
    #endregion

    #region Process
    private void ProcessReceive(SocketAsyncEventArgs socketAsyncEventArgs)
    {
        if (_socket == null)
        {
            Error(ChannelError.SocketIsNull);
            return;
        }
        if (!_socket.Connected)
        {
            Error(ChannelError.SocketNotConnect);
            return;
        }
        if (socketAsyncEventArgs.SocketError != SocketError.Success)
        {
            Logger.Debug($"Channel {ChannelId} Error {socketAsyncEventArgs.SocketError}");
            Error(ChannelError.SocketError);
            return;
        }
        if (socketAsyncEventArgs.BytesTransferred == 0)
        {
            Error(ChannelError.SocketBytesTransferredIsZero);
            return;
        }
        Receive?.Invoke(socketAsyncEventArgs.Buffer?.Skip(socketAsyncEventArgs.Offset).Take(socketAsyncEventArgs.BytesTransferred).ToArray());
        LastReceiveDateTime = DateTime.Now;
        StartReceive();
    }
    private void ProcessSend(SocketAsyncEventArgs socketAsyncEventArgs)
    {
        if (_socket == null)
        {
            Error(ChannelError.SocketIsNull);
            return;
        }
        if (!_socket.Connected)
        {
            Error(ChannelError.SocketNotConnect);
            return;
        }
        if (socketAsyncEventArgs.SocketError != SocketError.Success)
        {
            Error(ChannelError.SocketError);
            return;
        }
        LastSendDateTime = DateTime.Now;
    }
    private void ProcessDisconnect(SocketAsyncEventArgs socketAsyncEventArgs)
    {
        Logger.Info($"Channel {ChannelId} {RemoteEndPoint} disconnect");
        Release();
    }
    #endregion
}
