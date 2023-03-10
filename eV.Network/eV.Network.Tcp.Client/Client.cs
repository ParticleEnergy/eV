// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using eV.Module.EasyLog;
using eV.Network.Core;
using eV.Network.Core.Channel;
using eV.Network.Core.Interface;

namespace eV.Network.Tcp.Client;

public class Client : ITcpClient
{
    #region Event

    public event TcpChannelEvent? ConnectCompleted;
    public event TcpChannelEvent? DisconnectCompleted;

    #endregion

    #region Setting

    private IPEndPoint? _ipEndPoint;
    private int _receiveBufferSize;
    private bool _tcpKeepAlive;
    private string? _tlsTargetHost;
    private X509CertificateCollection? _tlsX509CertificateCollection;
    private bool _tlsCheckCertificateRevocation;

    #endregion


    #region Resource

    private Socket? _socket;
    private readonly ITcpChannel _channel;
    private readonly SocketAsyncEventArgs _connectSocketAsyncEventArgs;

    #endregion

    public Client(ClientSetting setting)
    {
        SetSetting(setting);
        ClientState = RunState.Off;

        SocketAsyncEventArgsCompleted socketAsyncEventArgsCompleted = new();
        socketAsyncEventArgsCompleted.ProcessConnect += ProcessConnect;
        socketAsyncEventArgsCompleted.ProcessDisconnect += ProcessDisconnect;

        if (_tlsTargetHost == null || _tlsX509CertificateCollection == null)
        {
            _channel = new TcpChannel(_receiveBufferSize);
        }
        else
        {
            _channel = new SslTcpChannel(_receiveBufferSize, _tlsTargetHost, _tlsX509CertificateCollection, _tlsCheckCertificateRevocation);
        }

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

        _tlsTargetHost = setting.TlsTargetHost;
        _tlsX509CertificateCollection = setting.TlsX509CertificateCollection;
        _tlsCheckCertificateRevocation = setting.TlsCheckCertificateRevocation;
    }

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
        _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, _tcpKeepAlive);
    }

    #endregion

    #region Process

    private bool ProcessConnect(SocketAsyncEventArgs socketAsyncEventArgs)
    {
        if (socketAsyncEventArgs.ConnectSocket is { Connected: true })
        {
            socketAsyncEventArgs.ConnectSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, _tcpKeepAlive);
            _channel.Open(socketAsyncEventArgs.ConnectSocket);

            return true;
        }

        Logger.Error($"Connect to Server {_ipEndPoint?.Address}:{_ipEndPoint?.Port} failed");
        return false;
    }

    private bool ProcessDisconnect(SocketAsyncEventArgs socketAsyncEventArgs)
    {
        Logger.Info($"Disconnect to Server {_ipEndPoint?.Address}:{_ipEndPoint?.Port}");
        Release();
        return true;
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
