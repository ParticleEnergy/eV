// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Sockets;
using eV.EasyLog;
using eV.Network.Core;
using eV.Network.Core.Interface;
namespace eV.Network.Security.Client;

public class Client : IClient
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
#if !NETSTANDARD
    private int _tcpKeepAliveTime;
    private int _tcpKeepAliveInterval;
    private int _tcpKeepAliveRetryCount;
#endif
    #endregion

    #region Resource
    private TcpClient? _tcpClient;
    private readonly SecurityChannel _channel;
    #endregion
    public Client(ClientSetting setting)
    {
        SetSetting(setting);
        ClientState = RunState.Off;

        _channel = new SecurityChannel(setting.TargetHost, setting.CertFile, setting.SslProtocols, setting.ReceiveBufferSize);
        _channel.OpenCompleted += OpenCompleted;
        _channel.CloseCompleted += CloseCompleted;
    }

    private void SetSetting(ClientSetting setting)
    {
        _ipEndPoint = new IPEndPoint(
            IPAddress.Parse(setting.Address),
            setting.Port
        );
#if !NETSTANDARD
        _tcpKeepAliveTime = setting.TcpKeepAliveTime;
        _tcpKeepAliveInterval = setting.TcpKeepAliveInterval;
        _tcpKeepAliveRetryCount = setting.TcpKeepAliveRetryCount;
#endif
    }

    public void Connect()
    {
        if (ClientState == RunState.On)
            return;
        try
        {
            ClientState = RunState.On;
            Init();
            Logger.Info($"Trying to connect to the server {_ipEndPoint?.Address}:{_ipEndPoint?.Port}");

            _channel.Open(_tcpClient!);
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
            if (_tcpClient is { Connected: true })
            {
                _tcpClient.Client.Shutdown(SocketShutdown.Both);
                Release();
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
        _tcpClient?.Client.Close();
        _tcpClient?.Close();
        if (_channel.ChannelState == RunState.On)
            _channel.Close();
        Logger.Info("Client released");
        DisconnectCompleted?.Invoke(_channel);
    }

    private void Init()
    {
        _tcpClient = new TcpClient(_ipEndPoint!.AddressFamily);
#if !NETSTANDARD
        _tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        _tcpClient.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, _tcpKeepAliveInterval);
        _tcpClient.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, _tcpKeepAliveRetryCount);
        _tcpClient.Client.SetSocketOption(SocketOptionLevel.Tcp,SocketOptionName.TcpKeepAliveTime,_tcpKeepAliveTime);
#endif
    }

    #region Channel
    private void OpenCompleted(IChannel channel)
    {
        if (channel.ChannelId.Equals(""))
        {
            Logger.Error($"Client {channel.RemoteEndPoint} open channel error by channelId is empty");
            return;
        }
        ConnectCompleted?.Invoke(channel);
        Logger.Info($"Connect to Server {_ipEndPoint?.Address}:{_ipEndPoint?.Port} success");
    }
    private void CloseCompleted(IChannel channel)
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
