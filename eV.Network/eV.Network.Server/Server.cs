// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using eV.Network.Core;
using log4net;
namespace eV.Network.Server
{
    public class Server
    {
        #region Event
        public event ChannelEvent? AcceptConnect;
        #endregion
        #region Public
        public RunState ServerState
        {
            get;
            private set;
        }
        #endregion

        #region Setting
        private IPEndPoint _ipEndPoint;
        private SocketType _socketType;
        private ProtocolType _protocolType;
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
        private readonly ObjectPool<Channel> _channelPool;
        private readonly ChannelManager _connectedChannels;
        private readonly Semaphore _maxAcceptedConnected;
        private readonly ILog _logger = LogManager.GetLogger(DefaultSetting.LoggerName);
        #endregion

        public Server(ServerSetting setting)
        {
            SetSetting(setting);
            ServerState = RunState.Off;

            _connectedCount = 0;

            _socket = new Socket(_ipEndPoint!.AddressFamily, _socketType, _protocolType);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            if (_protocolType == ProtocolType.Tcp)
            {
                _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, _tcpKeepAliveInterval);
                _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, _tcpKeepAliveRetryCount);
            }
            _socketAsyncEventArgsCompleted = new SocketAsyncEventArgsCompleted();
            _socketAsyncEventArgsCompleted.ProcessAccept += ProcessAccept;

            _acceptSocketAsyncEventArgsPool = new ObjectPool<SocketAsyncEventArgs>();
            _channelPool = new ObjectPool<Channel>();
            _maxAcceptedConnected = new Semaphore(_maxConnectionCount, _maxConnectionCount);
            _connectedChannels = new ChannelManager();
        }

        private void SetSetting(ServerSetting setting)
        {
            _ipEndPoint = new IPEndPoint(
                IPAddress.Parse(setting.Address),
                setting.Port
            );
            _socketType = setting.SocketType;
            _protocolType = setting.ProtocolType;
            _backlog = setting.Backlog;

            _maxConnectionCount = setting.MaxConnectionCount;
            _receiveBufferSize = setting.ReceiveBufferSize;

            _tcpKeepAliveTime = setting.TcpKeepAliveTime;
            _tcpKeepAliveInterval = setting.TcpKeepAliveInterval;
            _tcpKeepAliveRetryCount = setting.TcpKeepAliveRetryCount;
        }

        #region Operate
        public void Start()
        {
            if (ServerState == RunState.On)
            {
                _logger.Warn("The server is already turned on");
                return;
            }
            try
            {
                ServerState = RunState.On;
                Init();

                if (_ipEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    _socket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, false);
                    _socket.Bind(_ipEndPoint);
                }
                else
                {
                    _socket.Bind(_ipEndPoint);
                }

                _socket.Listen(_backlog);

                if (StartAccept())
                {
                    _logger.Info($"Server {_ipEndPoint.Address}:{_ipEndPoint.Port} start listen");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
            }
        }
        public void Stop()
        {
            if (ServerState == RunState.Off)
            {
                _logger.Warn("The server is already turned off");
                return;
            }
            ServerState = RunState.Off;
            try
            {
                _socket.Close();
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
            }
            finally
            {
                _logger.Info("Server stopping ...");
                Release();
            }
        }
        private void Release()
        {
            foreach (var channel in _connectedChannels.GetAllChannel())
            {
                channel.Value.Close();
            }
            while (_connectedCount > 0)
            {

            }
            _logger.Info("Server shutdown");
        }
        private void Init()
        {
            for (int i = 0; i < _backlog; ++i)
            {
                _acceptSocketAsyncEventArgsPool.Push(CreateAcceptSocketAsyncEventArgs());
            }

            for (int i = 0; i < _maxConnectionCount; ++i)
            {
                Channel channel = new(_receiveBufferSize);
                channel.OpenCompleted += OpenCompleted;
                channel.CloseCompleted += CloseCompleted;
                _channelPool.Push(channel);
            }
        }

        public Channel? GetConnectedChannel(string channelId)
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
            if (ServerState == RunState.Off)
            {
                return false;
            }
            SocketAsyncEventArgs? acceptSocketAsyncEventArgs;
            if (_acceptSocketAsyncEventArgsPool.Count > 1)
            {
                acceptSocketAsyncEventArgs = _acceptSocketAsyncEventArgsPool.Pop() ?? CreateAcceptSocketAsyncEventArgs();
            }
            else
            {
                acceptSocketAsyncEventArgs = CreateAcceptSocketAsyncEventArgs();
            }

            _maxAcceptedConnected.WaitOne();
            if (!_socket.AcceptAsync(acceptSocketAsyncEventArgs))
            {
                ProcessAccept(acceptSocketAsyncEventArgs);
            }
            return true;
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

        #region Process
        private void ProcessAccept(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            if (ServerState == RunState.Off)
            {
                return;
            }
            StartAccept();

            if (socketAsyncEventArgs.SocketError != SocketError.Success)
            {
                CloseAcceptSocketAsyncEventArgs(socketAsyncEventArgs);
                _logger.Error($"ProcessAccept {socketAsyncEventArgs.SocketError}");
                return;
            }

            if (socketAsyncEventArgs.AcceptSocket == null)
            {
                ResetAcceptSocketAsyncEventArgs(socketAsyncEventArgs);
                _logger.Error("ProcessAccept AcceptSocket is null");
                return;
            }

            if (!socketAsyncEventArgs.AcceptSocket.Connected)
            {
                _logger.Error("ProcessAccept AcceptSocket not Connected");
                ResetAcceptSocketAsyncEventArgs(socketAsyncEventArgs);
                return;
            }
            try
            {
                var channel = _channelPool.Pop();
                if (channel != null)
                {
                    if (socketAsyncEventArgs.AcceptSocket.ProtocolType == ProtocolType.Tcp)
                        socketAsyncEventArgs.AcceptSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, _tcpKeepAliveTime);
                    channel.Open(socketAsyncEventArgs.AcceptSocket);
                }
                else
                {
                    _logger.Error($"The maximum number of servers is {_maxConnectionCount}");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
            }
            finally
            {
                ResetAcceptSocketAsyncEventArgs(socketAsyncEventArgs);
            }
        }
        #endregion

        #region Channel
        private void OpenCompleted(Channel channel)
        {
            if (channel.ChannelId.Equals(""))
            {
                _logger.Error($"Open client {channel.RemoteEndPoint} channelId is empty");
                return;
            }
            if (_connectedChannels.Add(channel))
            {
                Interlocked.Increment(ref _connectedCount);
                AcceptConnect?.Invoke(channel);
                _logger.Info($"Client {channel.RemoteEndPoint} connected");
            }
            else
            {
                _logger.Error($"Channel {channel.RemoteEndPoint} {channel.ChannelId} add on failed");
            }
        }
        private void CloseCompleted(Channel channel)
        {
            if (channel.ChannelId.Equals(""))
            {
                _logger.Error($"Close client {channel.RemoteEndPoint} channelId is empty");
                return;
            }
            if (!_connectedChannels.Remove(channel))
            {
                _logger.Error($"Channel {channel.RemoteEndPoint} {channel.ChannelId} remove on failed");
            }
            _channelPool.Push(channel);
            Interlocked.Decrement(ref _connectedCount);
            _maxAcceptedConnected.Release();
        }
        #endregion
    }
}
