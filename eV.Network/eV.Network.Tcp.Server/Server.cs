// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Sockets;
using eV.Module.EasyLog;
using eV.Network.Core;
using eV.Network.Core.Channel;
using eV.Network.Core.Interface;
namespace eV.Network.Tcp.Server;

public class Server : IServer
{
    #region Public
    public RunState ServerState
    {
        get;
        private set;
    }
    #endregion

    #region Event
    public event TcpChannelEvent? AcceptConnect;
    #endregion

    #region Setting
    private IPEndPoint _ipEndPoint;
    private int _backlog;
    private int _maxConnectionCount;
    private int _receiveBufferSize;
    private int _tcpKeepAliveTime;
    private int _tcpKeepAliveInterval;
    private int _tcpKeepAliveRetryCount;
    #endregion

    #region Resource
    private int _connectedCount;
    private readonly Socket _socket;
    private readonly SocketAsyncEventArgsCompleted _socketAsyncEventArgsCompleted;
    private readonly ObjectPool<SocketAsyncEventArgs> _acceptSocketAsyncEventArgsPool;
    private readonly ObjectPool<TcpChannel> _channelPool;
    private readonly ChannelManager _connectedChannels;
    private readonly Semaphore _maxAcceptedConnected;
    #endregion

    #region Construct
    public Server(ServerSetting setting)
    {
        SetSetting(setting);
        ServerState = RunState.Off;

        _connectedCount = 0;

        _socket = new Socket(_ipEndPoint!.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, _tcpKeepAliveInterval);
        _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, _tcpKeepAliveRetryCount);

        _socketAsyncEventArgsCompleted = new SocketAsyncEventArgsCompleted();
        _socketAsyncEventArgsCompleted.ProcessAccept += ProcessAccept;

        _acceptSocketAsyncEventArgsPool = new ObjectPool<SocketAsyncEventArgs>();
        _channelPool = new ObjectPool<TcpChannel>();
        _maxAcceptedConnected = new Semaphore(_maxConnectionCount + 1, _maxConnectionCount + 1);
        _connectedChannels = new ChannelManager();
    }

    private void SetSetting(ServerSetting setting)
    {
        _ipEndPoint = new IPEndPoint(
            IPAddress.Parse(setting.Host),
            setting.Port
        );
        _backlog = setting.Backlog;

        _maxConnectionCount = setting.MaxConnectionCount;
        _receiveBufferSize = setting.ReceiveBufferSize;

        _tcpKeepAliveTime = setting.TcpKeepAliveTime;
        _tcpKeepAliveInterval = setting.TcpKeepAliveInterval;
        _tcpKeepAliveRetryCount = setting.TcpKeepAliveRetryCount;
    }
    #endregion

    #region Process
    private void ProcessAccept(SocketAsyncEventArgs socketAsyncEventArgs)
    {
        if (ServerState == RunState.Off)
            return;
        StartAccept();

        if (socketAsyncEventArgs.SocketError != SocketError.Success)
        {
            CloseAcceptSocketAsyncEventArgs(socketAsyncEventArgs);
            Logger.Error($"ProcessAccept {socketAsyncEventArgs.SocketError}");
            return;
        }

        if (socketAsyncEventArgs.AcceptSocket == null)
        {
            ResetAcceptSocketAsyncEventArgs(socketAsyncEventArgs);
            Logger.Error("ProcessAccept AcceptSocket is null");
            return;
        }

        if (!socketAsyncEventArgs.AcceptSocket.Connected)
        {
            Logger.Error("ProcessAccept AcceptSocket not Connected");
            ResetAcceptSocketAsyncEventArgs(socketAsyncEventArgs);
            return;
        }
        try
        {
            TcpChannel? channel = _channelPool.Pop();
            if (channel != null)
            {
                socketAsyncEventArgs.AcceptSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, _tcpKeepAliveTime);
                channel.Open(socketAsyncEventArgs.AcceptSocket);
            }
            else
            {
                Logger.Error($"The maximum number of servers is {_maxConnectionCount}");
            }
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
        finally
        {
            ResetAcceptSocketAsyncEventArgs(socketAsyncEventArgs);
        }
    }
    #endregion

    #region Operate
    public void Start()
    {
        if (ServerState == RunState.On)
        {
            Logger.Warn("The server is already turned on");
            return;
        }
        try
        {
            ServerState = RunState.On;
            Init();

            if (_ipEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
            {
                _socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
                _socket.Bind(_ipEndPoint);
            }
            else
            {
                _socket.Bind(_ipEndPoint);
            }

            _socket.Listen(_backlog);

            if (StartAccept())
                Logger.Info($"Server {_ipEndPoint.Address}:{_ipEndPoint.Port} start listen");
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }
    public void Stop()
    {
        if (ServerState == RunState.Off)
        {
            Logger.Warn("The server is already turned off");
            return;
        }
        ServerState = RunState.Off;
        try
        {
            _socket.Close();
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
        finally
        {
            Logger.Info("Server stopping ...");
            Release();
        }
    }
    private void Release()
    {
        foreach (KeyValuePair<string, IChannel> channel in _connectedChannels.GetAllChannel())
            channel.Value.Close();
        Logger.Info("Server shutdown");
    }
    private void Init()
    {
        for (int i = 0; i < _backlog; ++i)
            _acceptSocketAsyncEventArgsPool.Push(CreateAcceptSocketAsyncEventArgs());

        for (int i = 0; i < _maxConnectionCount; ++i)
        {
            TcpChannel channel = new(_receiveBufferSize);
            channel.OpenCompleted += OpenCompleted;
            channel.CloseCompleted += CloseCompleted;
            _channelPool.Push(channel);
        }
    }

    public IChannel? GetConnectedChannel(string channelId)
    {
        return _connectedChannels.GetChannel(channelId);
    }
    public ChannelManager GetConnectedAllChannels()
    {
        return _connectedChannels;
    }
    #endregion

    #region Accept
    private bool StartAccept()
    {
        try
        {
            _maxAcceptedConnected.WaitOne();

            if (ServerState == RunState.Off)
                return false;

            SocketAsyncEventArgs? acceptSocketAsyncEventArgs;
            if (_acceptSocketAsyncEventArgsPool.Count > 1)
                acceptSocketAsyncEventArgs = _acceptSocketAsyncEventArgsPool.Pop() ?? CreateAcceptSocketAsyncEventArgs();
            else
                acceptSocketAsyncEventArgs = CreateAcceptSocketAsyncEventArgs();

            if (!_socket.AcceptAsync(acceptSocketAsyncEventArgs))
                ProcessAccept(acceptSocketAsyncEventArgs);
            return true;
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return false;
        }
    }
    private SocketAsyncEventArgs CreateAcceptSocketAsyncEventArgs()
    {
        SocketAsyncEventArgs acceptSocketAsyncEventArgs = new();
        acceptSocketAsyncEventArgs.Completed += _socketAsyncEventArgsCompleted.OnCompleted;
        return acceptSocketAsyncEventArgs;
    }
    private void CloseAcceptSocketAsyncEventArgs(SocketAsyncEventArgs acceptSocketAsyncEventArgs)
    {
        acceptSocketAsyncEventArgs.AcceptSocket?.Close();
        ResetAcceptSocketAsyncEventArgs(acceptSocketAsyncEventArgs);
    }
    private void ResetAcceptSocketAsyncEventArgs(SocketAsyncEventArgs acceptSocketAsyncEventArgs)
    {
        acceptSocketAsyncEventArgs.AcceptSocket = null;
        _acceptSocketAsyncEventArgsPool.Push(acceptSocketAsyncEventArgs);
    }
    #endregion

    #region Channel
    private void OpenCompleted(ITcpChannel channel)
    {
        if (channel.ChannelId.Equals(""))
        {
            Logger.Error($"Open client {channel.RemoteEndPoint} channelId is empty");
            return;
        }
        if (_connectedChannels.Add(channel))
        {
            Interlocked.Increment(ref _connectedCount);
            AcceptConnect?.Invoke(channel);
            Logger.Info($"Client {channel.RemoteEndPoint} connected");
        }
        else
        {
            Logger.Error($"Channel {channel.RemoteEndPoint} {channel.ChannelId} add on failed");
        }
    }
    private void CloseCompleted(ITcpChannel channel)
    {
        if (channel.ChannelId.Equals(""))
        {
            Logger.Error($"Close client {channel.RemoteEndPoint} channelId is empty");
            return;
        }
        if (!_connectedChannels.Remove(channel))
            Logger.Error($"Channel {channel.RemoteEndPoint} {channel.ChannelId} remove on failed");
        _channelPool.Push((TcpChannel)channel);
        Interlocked.Decrement(ref _connectedCount);
        _maxAcceptedConnected.Release();
    }
    #endregion
}
