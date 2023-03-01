// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using eV.Module.EasyLog;
using eV.Network.Core;
using eV.Network.Core.Channel;
using eV.Network.Core.Interface;

namespace eV.Network.Tcp.Client;

public class Client : ITcpClient
{
    public Client(ClientSetting setting)
    {
        SetSetting(setting);
        ClientState = RunState.Off;

        SocketAsyncEventArgsCompleted socketAsyncEventArgsCompleted = new();
        socketAsyncEventArgsCompleted.ProcessConnect += ProcessConnect;
        socketAsyncEventArgsCompleted.ProcessDisconnect += ProcessDisconnect;

        _channel = new TcpChannel(_receiveBufferSize);
        _channel.OpenCompleted += OpenCompleted;
        _channel.CloseCompleted += CloseCompleted;

        _connectSocketAsyncEventArgs = new SocketAsyncEventArgs();
        _connectSocketAsyncEventArgs.Completed += socketAsyncEventArgsCompleted.OnCompleted;
        _connectSocketAsyncEventArgs.RemoteEndPoint = _ipEndPoint;
        _connectSocketAsyncEventArgs.DisconnectReuseSocket = false;
    }

    #region Public

    public RunState ClientState { get; private set; }

    #endregion

    private void SetSetting(ClientSetting setting)
    {
        _ipEndPoint = new IPEndPoint(
            IPAddress.Parse(setting.Host),
            setting.Port
        );
        _receiveBufferSize = setting.ReceiveBufferSize;
        _tcpKeepAlive = setting.TcpKeepAlive;
        _tcpKeepAliveTime = setting.TcpKeepAliveTime;
        _tcpKeepAliveInterval = setting.TcpKeepAliveInterval;
    }

    #region Event

    public event TcpChannelEvent? ConnectCompleted;
    public event TcpChannelEvent? DisconnectCompleted;

    #endregion

    #region Setting

    private IPEndPoint? _ipEndPoint;
    private int _receiveBufferSize;
    private bool _tcpKeepAlive;
    private int _tcpKeepAliveTime;
    private int _tcpKeepAliveInterval;
    private const uint SioKeepaliveValue = 0x98000004;

    #endregion


    #region Resource

    private Socket? _socket;
    private readonly TcpChannel _channel;
    private readonly SocketAsyncEventArgs _connectSocketAsyncEventArgs;

    #endregion

    #region Operate

    public void Connect()
    {
        if (ClientState == RunState.On)
            return;
        try
        {
            ClientState = RunState.On;
            Init();
            Logger.Info($"Trying to connect to the server {_ipEndPoint?.Address}:{_ipEndPoint?.Port}");
            if (!_socket!.ConnectAsync(_connectSocketAsyncEventArgs))
                ProcessConnect(_connectSocketAsyncEventArgs);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }

    public void Disconnect()
    {
        if (ClientState == RunState.Off)
            return;
        ClientState = RunState.Off;
        try
        {
            if (_socket is { Connected: true })
            {
                _socket.Shutdown(SocketShutdown.Both);
                if (!_socket.DisconnectAsync(_connectSocketAsyncEventArgs))
                    ProcessDisconnect(_connectSocketAsyncEventArgs);
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

    private void Release()
    {
        _socket?.Close();
        if (_channel.ChannelState == RunState.On)
            _channel.Close();
        Logger.Info("Client released");
        DisconnectCompleted?.Invoke(_channel);
    }

    private void Init()
    {
        _socket = new Socket(_ipEndPoint!.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        if (!_tcpKeepAlive) return;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            byte[] inOptionValues = new byte[12];
            BitConverter.GetBytes((uint)1).CopyTo(inOptionValues, 1);
            BitConverter.GetBytes((uint)_tcpKeepAliveTime * 1000).CopyTo(inOptionValues, 4);
            BitConverter.GetBytes((uint)_tcpKeepAliveInterval * 1000).CopyTo(inOptionValues, 8);
            _socket.IOControl(unchecked((int)SioKeepaliveValue), inOptionValues, null);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || Environment.OSVersion.VersionString.ToLower().Contains("android"))
        {
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            _socket.SetSocketOption(SocketOptionLevel.Tcp, (SocketOptionName)3, _tcpKeepAliveTime);
            _socket.SetSocketOption(SocketOptionLevel.Tcp, (SocketOptionName)17, _tcpKeepAliveInterval);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
#if !NETSTANDARD
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, _tcpKeepAliveTime);
            _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, _tcpKeepAliveInterval);
#endif
        }
        else if (Environment.OSVersion.VersionString.ToLower().Contains("darwin"))
        {
            _socket.SetSocketOption(SocketOptionLevel.Tcp, (SocketOptionName)0x10, _tcpKeepAliveTime);
            _socket.SetSocketOption(SocketOptionLevel.Tcp, (SocketOptionName)0x101, _tcpKeepAliveInterval);
        }
    }

    #endregion

    #region Process

    private void ProcessConnect(SocketAsyncEventArgs socketAsyncEventArgs)
    {
        if (socketAsyncEventArgs.ConnectSocket is { Connected: true })
        {
#if !NETSTANDARD
            if (socketAsyncEventArgs.ConnectSocket.ProtocolType == ProtocolType.Tcp)
                socketAsyncEventArgs.ConnectSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, _tcpKeepAliveTime);
#endif
            _channel.Open(socketAsyncEventArgs.ConnectSocket);
        }
        else
        {
            Logger.Error($"Connect to Server {_ipEndPoint?.Address}:{_ipEndPoint?.Port} failed");
        }
    }

    private void ProcessDisconnect(SocketAsyncEventArgs socketAsyncEventArgs)
    {
        Logger.Info($"Disconnect to Server {_ipEndPoint?.Address}:{_ipEndPoint?.Port}");
        Release();
    }

    #endregion

    #region Channel

    private void OpenCompleted(ITcpChannel channel)
    {
        if (channel.ChannelId.Equals(""))
        {
            Logger.Error($"Client {channel.RemoteEndPoint} open channel error by channelId is empty");
            return;
        }

        ConnectCompleted?.Invoke(channel);
        Logger.Info($"Connect to Server {_ipEndPoint?.Address}:{_ipEndPoint?.Port} success");
    }

    private void CloseCompleted(ITcpChannel channel)
    {
        if (channel.ChannelId.Equals(""))
        {
            Logger.Error($"Client {channel.RemoteEndPoint} close channel error by channelId is empty");
            return;
        }

        if (ClientState == RunState.On)
            Disconnect();
    }

    #endregion
}
