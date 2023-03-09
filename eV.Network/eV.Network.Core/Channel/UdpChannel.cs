// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Sockets;
using eV.Module.EasyLog;
using eV.Network.Core.Interface;

namespace eV.Network.Core.Channel;

public class UdpChannel : IUdpChannel
{
    #region Public

    public RunState ChannelState { get; private set; }
    public string ChannelId { get; }
    public EndPoint? RemoteEndPoint => null;

    #endregion

    #region Time

    public DateTime? ConnectedDateTime { get; private set; }
    public DateTime? LastReceiveDateTime { get; private set; }
    public DateTime? LastSendDateTime { get; private set; }

    #endregion

    #region Resource

    private Socket? _socket;
    private readonly SocketAsyncEventArgsCompleted _socketAsyncEventArgsCompleted;
    private readonly ObjectPool<SocketAsyncEventArgs> _sendSocketAsyncEventArgsPool;
    private readonly SocketAsyncEventArgs _receiveSocketAsyncEventArgs;
    private readonly byte[] _receiveBuffer;
    private readonly EndPoint _broadcastEndPoint;
    private readonly EndPoint _multiCastEndPoint;

    #endregion

    #region Event

    public event UdpChannelEvent? OpenCompleted;
    public event UdpChannelEvent? CloseCompleted;

    #endregion

    #region Construct

    public UdpChannel(int receiveBufferSize, EndPoint broadcastEndPoint, EndPoint multiCastEndPoint)
    {
        ChannelId = Guid.NewGuid().ToString();
        ChannelState = RunState.Off;

        _broadcastEndPoint = broadcastEndPoint;
        _multiCastEndPoint = multiCastEndPoint;

        // Completed
        _socketAsyncEventArgsCompleted = new SocketAsyncEventArgsCompleted();
        _socketAsyncEventArgsCompleted.ProcessReceiveFrom += ProcessReceiveFrom;
        _socketAsyncEventArgsCompleted.ProcessSendTo += ProcessSendTo;

        // Receive
        _receiveBuffer = new byte[receiveBufferSize];
        _receiveSocketAsyncEventArgs = new SocketAsyncEventArgs();
        _receiveSocketAsyncEventArgs.Completed += _socketAsyncEventArgsCompleted.OnCompleted;
        _receiveSocketAsyncEventArgs.SetBuffer(_receiveBuffer, 0, receiveBufferSize);
        _receiveSocketAsyncEventArgs.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        // Send
        _sendSocketAsyncEventArgsPool = new ObjectPool<SocketAsyncEventArgs>();
    }

    #endregion

    #region Operate

    public void Open(Socket socket, int maxConcurrentSend)
    {
        if (ChannelState == RunState.On)
        {
            Logger.Warn("The channel is already turned on");
            return;
        }

        try
        {
            Init(socket, maxConcurrentSend);

            OpenCompleted?.Invoke(this);
            Logger.Info($"Channel {ChannelId} {RemoteEndPoint} open");
            Logger.Info(StartReceiveFrom()
                ? $"Channel {ChannelId} {RemoteEndPoint} start receive"
                : $"Channel {ChannelId} {RemoteEndPoint} start receive failed");
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            Close();
        }
    }

    public void Close()
    {
        if (ChannelState == RunState.Off)
            return;
        ChannelState = RunState.Off;
        try
        {
            _socket = null;
            do
            {
                _sendSocketAsyncEventArgsPool.Pop();
            } while (_sendSocketAsyncEventArgsPool.Count > 0);

            Array.Clear(_receiveBuffer, 0, _receiveBuffer.Length);

            Logger.Info($"Channel {ChannelId} {RemoteEndPoint} close");
            CloseCompleted?.Invoke(this);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }

    /// <summary>
    /// 重置Channel
    /// </summary>
    private void Init(Socket socket, int maxConcurrentSend)
    {
        for (int i = 0; i < maxConcurrentSend; ++i)
        {
            SocketAsyncEventArgs socketAsyncEventArgs = new();
            socketAsyncEventArgs.Completed += _socketAsyncEventArgsCompleted.OnCompleted;
            _sendSocketAsyncEventArgsPool.Push(socketAsyncEventArgs);
        }

        _socket = socket;
        ChannelState = RunState.On;
        ConnectedDateTime = DateTime.Now;
    }

    #endregion

    #region IO

    private bool StartReceiveFrom()
    {
        if (ChannelState == RunState.Off)
            return false;
        if (_socket == null)
        {
            ChannelError.Error(ChannelError.ErrorCode.SocketIsNull, Close);
            return false;
        }

        if (!_socket.ReceiveFromAsync(_receiveSocketAsyncEventArgs))
            ProcessReceiveFrom(_receiveSocketAsyncEventArgs);
        return true;
    }

    public Action<byte[]?, EndPoint?>? Receive { get; set; }

    public bool SendBroadcast(byte[] data)
    {
        return Send(data, _broadcastEndPoint);
    }

    public bool SendMulticast(byte[] data)
    {
        return Send(data, _multiCastEndPoint);
    }

    public bool Send(byte[] data, EndPoint? destEndPoint)
    {
        if (ChannelState == RunState.Off)
            return false;
        if (_socket == null)
        {
            ChannelError.Error(ChannelError.ErrorCode.SocketIsNull, Close);
            return false;
        }

        SocketAsyncEventArgs? socketAsyncEventArgs = _sendSocketAsyncEventArgsPool.Pop();
        if (socketAsyncEventArgs == null)
        {
            Logger.Error("Maximum concurrent send quantity exceeded");
            return false;
        }

        socketAsyncEventArgs.SetBuffer(data, 0, data.Length);
        socketAsyncEventArgs.RemoteEndPoint = destEndPoint;

        if (!_socket!.SendToAsync(socketAsyncEventArgs))
            ProcessSendTo(socketAsyncEventArgs);
        return true;
    }

    #endregion

    #region Process

    private bool ProcessReceiveFrom(SocketAsyncEventArgs socketAsyncEventArgs)
    {
        try
        {
            if (_socket == null)
            {
                ChannelError.Error(ChannelError.ErrorCode.SocketIsNull, Close);
                return false;
            }

            if (socketAsyncEventArgs.SocketError != SocketError.Success)
            {
                Logger.Debug($"Channel {ChannelId} Error {socketAsyncEventArgs.SocketError}");
                ChannelError.Error(ChannelError.ErrorCode.SocketError, Close);
                return false;
            }

            if (socketAsyncEventArgs.BytesTransferred == 0)
            {
                ChannelError.Error(ChannelError.ErrorCode.SocketBytesTransferredIsZero, Close);
                return false;
            }

            Receive?.Invoke(
                socketAsyncEventArgs.Buffer?
                    .Skip(socketAsyncEventArgs.Offset)
                    .Take(socketAsyncEventArgs.BytesTransferred)
                    .ToArray(),
                socketAsyncEventArgs.RemoteEndPoint
            );
            LastReceiveDateTime = DateTime.Now;
            StartReceiveFrom();
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            Close();
            return false;
        }

        return true;
    }

    private bool ProcessSendTo(SocketAsyncEventArgs socketAsyncEventArgs)
    {
        try
        {
            if (_socket == null)
            {
                ChannelError.Error(ChannelError.ErrorCode.SocketIsNull, Close);
                return false;
            }

            if (socketAsyncEventArgs.SocketError != SocketError.Success)
            {
                ChannelError.Error(ChannelError.ErrorCode.SocketError, Close);
                return false;
            }

            LastSendDateTime = DateTime.Now;
            socketAsyncEventArgs.RemoteEndPoint = null;
            _sendSocketAsyncEventArgsPool.Push(socketAsyncEventArgs);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return false;
        }

        return true;
    }

    #endregion
}
