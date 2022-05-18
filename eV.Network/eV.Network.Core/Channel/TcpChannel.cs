// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Sockets;
using eV.EasyLog;
using eV.Network.Core.Interface;
namespace eV.Network.Core.Channel;

public class TcpChannel : ITcpChannel
{
    public TcpChannel(int receiveBufferSize)
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

    #region Event
    public event TcpChannelEvent? OpenCompleted;
    public event TcpChannelEvent? CloseCompleted;
    #endregion

    #region Public
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
        try
        {
            _socket?.Close();
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
        finally
        {
            _socket = null;
            _receiveSocketAsyncEventArgs.AcceptSocket = null;
            _sendSocketAsyncEventArgs.AcceptSocket = null;
            Array.Clear(_receiveBuffer, 0, _receiveBuffer.Length);

            Logger.Info($"Channel {ChannelId} {RemoteEndPoint} close");
            CloseCompleted?.Invoke(this);
        }
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
            ChannelError.Error(ChannelError.ErrorCode.SocketIsNull, Close);
            return false;
        }
        if (!_socket.Connected)
        {
            ChannelError.Error(ChannelError.ErrorCode.SocketNotConnect, Close);
            return false;
        }
        if (!_socket.ReceiveAsync(_receiveSocketAsyncEventArgs))
            ProcessReceive(_receiveSocketAsyncEventArgs);
        return true;
    }
    public Action<byte[]?>? Receive { get; set; }

    public bool Send(byte[] data)
    {
        if (ChannelState == RunState.Off)
            return false;
        if (_socket == null)
        {
            ChannelError.Error(ChannelError.ErrorCode.SocketIsNull, Close);
            return false;
        }
        if (!_socket.Connected)
        {
            ChannelError.Error(ChannelError.ErrorCode.SocketNotConnect, Close);
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
            ChannelError.Error(ChannelError.ErrorCode.SocketIsNull, Close);
            return;
        }
        if (!_socket.Connected)
        {
            ChannelError.Error(ChannelError.ErrorCode.SocketNotConnect, Close);
            return;
        }
        if (socketAsyncEventArgs.SocketError != SocketError.Success)
        {
            Logger.Debug($"Channel {ChannelId} Error {socketAsyncEventArgs.SocketError}");
            ChannelError.Error(ChannelError.ErrorCode.SocketError, Close);

            return;
        }
        if (socketAsyncEventArgs.BytesTransferred == 0)
        {
            ChannelError.Error(ChannelError.ErrorCode.SocketBytesTransferredIsZero, Close);
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
            ChannelError.Error(ChannelError.ErrorCode.SocketIsNull, Close);
            return;
        }
        if (!_socket.Connected)
        {
            ChannelError.Error(ChannelError.ErrorCode.SocketNotConnect, Close);
            return;
        }
        if (socketAsyncEventArgs.SocketError != SocketError.Success)
        {
            ChannelError.Error(ChannelError.ErrorCode.SocketError, Close);
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
