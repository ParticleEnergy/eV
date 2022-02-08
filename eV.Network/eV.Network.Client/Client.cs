// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using System.Net.Sockets;
using eV.Network.Core;
using log4net;
namespace eV.Network.Client
{

    public class Client
    {
        #region Event
        public event ChannelEvent? ConnectCompleted;
        public event ChannelEvent? DisconnectCompleted;
        #endregion

        #region Public
        public RunState ClientState
        {
            get;
            private set;
        }
        #endregion

        #region Setting
        private IPEndPoint? _ipEndPoint;
        private SocketType _socketType;
        private ProtocolType _protocolType;
        private int _receiveBufferSize;
        #endregion


        #region Resource
        private Socket? _socket;
        private readonly Channel _channel;
        private readonly SocketAsyncEventArgs _connectSocketAsyncEventArgs;
        private readonly ILog _logger = LogManager.GetLogger(DefaultSetting.LoggerName);
        #endregion
        public Client(ClientSetting setting)
        {
            SetSetting(setting);
            ClientState = RunState.Off;

            SocketAsyncEventArgsCompleted socketAsyncEventArgsCompleted = new();
            socketAsyncEventArgsCompleted.ProcessConnect += ProcessConnect;
            socketAsyncEventArgsCompleted.ProcessDisconnect += ProcessDisconnect;

            _channel = new Channel(_receiveBufferSize);
            _channel.OpenCompleted += OpenCompleted;
            _channel.CloseCompleted += CloseCompleted;

            _connectSocketAsyncEventArgs = new SocketAsyncEventArgs();
            _connectSocketAsyncEventArgs.Completed += socketAsyncEventArgsCompleted.OnCompleted;
            _connectSocketAsyncEventArgs.RemoteEndPoint = _ipEndPoint;
            _connectSocketAsyncEventArgs.DisconnectReuseSocket = false;
        }
        private void SetSetting(ClientSetting setting)
        {
            _ipEndPoint = new IPEndPoint(
                IPAddress.Parse(setting.Address),
                setting.Port
            );
            _socketType = setting.SocketType;
            _protocolType = setting.ProtocolType;
            _receiveBufferSize = setting.ReceiveBufferSize;
        }

        #region Operate
        public void Connect()
        {
            if (ClientState == RunState.On)
            {
                return;
            }
            try
            {
                ClientState = RunState.On;
                Init();
                _logger.Info($"Trying to connect to the server {_ipEndPoint?.Address}:{_ipEndPoint?.Port}");
                if (!_socket!.ConnectAsync(_connectSocketAsyncEventArgs))
                {
                    ProcessConnect(_connectSocketAsyncEventArgs);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
            }
        }
        public void Disconnect()
        {
            if (ClientState == RunState.Off)
            {
                return;
            }
            ClientState = RunState.Off;
            try
            {
                if (_socket is { Connected: true })
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    if (!_socket.DisconnectAsync(_connectSocketAsyncEventArgs))
                    {
                        ProcessDisconnect(_connectSocketAsyncEventArgs);
                    }
                }
                else
                {
                    Release();
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
            }
        }
        private void Release()
        {
            _socket?.Close();
            if (_channel.ChannelState == RunState.On)
            {
                _channel.Close();
            }
            _logger.Info("Client released");
            DisconnectCompleted?.Invoke(_channel);
        }
        private void Init()
        {
            _socket = new Socket(_ipEndPoint!.AddressFamily, _socketType, _protocolType);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        }
        #endregion

        #region Process
        private void ProcessConnect(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            if (socketAsyncEventArgs.ConnectSocket != null)
            {
                _channel.Open(socketAsyncEventArgs.ConnectSocket);
            }
            else
            {
                _logger.Error($"Connect to Server {_ipEndPoint?.Address}:{_ipEndPoint?.Port} failed");
            }
        }
        private void ProcessDisconnect(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            _logger.Info($"Disconnect to Server {_ipEndPoint?.Address}:{_ipEndPoint?.Port}");
            Release();
        }
        #endregion

        #region Channel
        private void OpenCompleted(Channel channel)
        {
            if (channel.ChannelId.Equals(""))
            {
                _logger.Error($"Client {channel.RemoteEndPoint} open channel error by channelId is empty");
                return;
            }
            ConnectCompleted?.Invoke(channel);
            _logger.Info($"Connect to Server {_ipEndPoint?.Address}:{_ipEndPoint?.Port} success");
        }
        private void CloseCompleted(Channel channel)
        {
            if (channel.ChannelId.Equals(""))
            {
                _logger.Error($"Client {channel.RemoteEndPoint} close channel error by channelId is empty");
                return;
            }
            if (ClientState == RunState.On)
            {
                Disconnect();
            }
        }
        #endregion
    }
}
