// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using eV.Module.EasyLog;
using eV.Network.Core;
using eV.Network.Core.Channel;
using eV.Network.Core.Interface;

namespace eV.Network.Tcp.Security.Client;

public class Client : ITcpClient
{
    #region Event

    public event TcpChannelEvent? ConnectCompleted;
    public event TcpChannelEvent? DisconnectCompleted;

    #endregion

    #region Public

    public RunState ClientState { get; private set; }

    #endregion

    #region Setting

    private IPEndPoint? _ipEndPoint;

    private bool _tcpKeepAlive;
    private int _tcpKeepAliveTime;
    private int _tcpKeepAliveInterval;
    private const uint SioKeepaliveValue = 0x98000004;

    #endregion

    #region Resource

    private TcpClient? _tcpClient;
    private readonly TcpSecurityChannel _channel;

    #endregion

    public Client(ClientSetting setting)
    {
        SetSetting(setting);
        ClientState = RunState.Off;

        _channel = new TcpSecurityChannel(setting.TargetHost, setting.CertFile, setting.SslProtocols,
            setting.ReceiveBufferSize);
        _channel.OpenCompleted += OpenCompleted;
        _channel.CloseCompleted += CloseCompleted;
    }

    private void SetSetting(ClientSetting setting)
    {
        _ipEndPoint = new IPEndPoint(
            IPAddress.Parse(setting.Host),
            setting.Port
        );

        _tcpKeepAlive = setting.TcpKeepAlive;
        _tcpKeepAliveTime = setting.TcpKeepAliveTime;
        _tcpKeepAliveInterval = setting.TcpKeepAliveInterval;
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

        if (!_tcpKeepAlive) return;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            byte[] inOptionValues = new byte[12];
            BitConverter.GetBytes((uint)1).CopyTo(inOptionValues, 1);
            BitConverter.GetBytes((uint)_tcpKeepAliveTime * 1000).CopyTo(inOptionValues, 4);
            BitConverter.GetBytes((uint)_tcpKeepAliveInterval * 1000).CopyTo(inOptionValues, 8);
            _tcpClient.Client.IOControl(unchecked((int)SioKeepaliveValue), inOptionValues, null);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || Environment.OSVersion.VersionString.ToLower().Contains("android"))
        {
            _tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            _tcpClient.Client.SetSocketOption(SocketOptionLevel.Tcp, (SocketOptionName)3, _tcpKeepAliveTime);
            _tcpClient.Client.SetSocketOption(SocketOptionLevel.Tcp, (SocketOptionName)17, _tcpKeepAliveInterval);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
#if !NETSTANDARD
            _tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            _tcpClient.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, _tcpKeepAliveTime);
            _tcpClient.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, _tcpKeepAliveInterval);
#endif
        }
        else if (Environment.OSVersion.VersionString.ToLower().Contains("darwin"))
        {
            _tcpClient.Client.SetSocketOption(SocketOptionLevel.Tcp, (SocketOptionName)0x10, _tcpKeepAliveTime);
            _tcpClient.Client.SetSocketOption(SocketOptionLevel.Tcp, (SocketOptionName)0x101, _tcpKeepAliveInterval);
        }
    }

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
