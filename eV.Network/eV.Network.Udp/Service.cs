// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.


using System.Net;
using System.Net.Sockets;
using eV.EasyLog;
using eV.Network.Core;
using eV.Network.Core.Channel;
using eV.Network.Core.Interface;
namespace eV.Network.Udp;

public class Service : IServer
{
    #region Public
    public RunState ServerState
    {
        get;
        private set;
    }
    #endregion

    #region Setting
    private IPEndPoint? _ipEndPoint;
    private IPEndPoint? _multiCastEndPoint;
    private IPEndPoint? _broadcastEndPoint;
    private MulticastOption? _multicastOption;
    private int _receiveBufferSize;
    private int _multicastTimeToLive;
    #endregion

    #region Resource
    private readonly Socket _socket;
    private readonly UdpChannel _channel;
    #endregion

    #region Event
    public event UdpChannelEvent? OnBind;
    public event Action? OnRelease;
    #endregion

    #region Construct
    public Service(ServerSetting setting)
    {
        SetSetting(setting);
        ServerState = RunState.Off;

        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // 广播
        _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
        //设置多播
        _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);
        _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, _multicastOption!);
        // IP 多路广播生存时间
        _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, _multicastTimeToLive);

        _channel = new UdpChannel(_receiveBufferSize, _broadcastEndPoint!, _multiCastEndPoint!);
        _channel.OpenCompleted += OpenCompleted;
    }

    private void SetSetting(ServerSetting setting)
    {
        _broadcastEndPoint = new IPEndPoint(
            IPAddress.Broadcast,
            setting.Port
        );
        _multiCastEndPoint = new IPEndPoint(
            IPAddress.Parse(setting.MultiCastHost),
            setting.MultiCastPort
        );
        _ipEndPoint = new IPEndPoint(
            IPAddress.Parse(setting.Host),
            setting.Port
        );
        _multicastOption = new MulticastOption(IPAddress.Parse(setting.MultiCastHost), IPAddress.Parse(setting.Host));
        _receiveBufferSize = setting.ReceiveBufferSize;
        _multicastTimeToLive = setting.MulticastTimeToLive;
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

            if (_ipEndPoint!.AddressFamily == AddressFamily.InterNetworkV6)
            {
                _socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
                _socket.Bind(_ipEndPoint);
            }
            else
            {
                _socket.Bind(_ipEndPoint);
            }

            _channel.Open(_socket);
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
            _channel.Close();
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
        OnRelease?.Invoke();
    }
    private void Init()
    {
        _channel.Open(_socket);
    }
    #endregion

    #region Channel
    private void OpenCompleted(IUdpChannel channel)
    {
        OnBind?.Invoke(channel);
    }
    #endregion
}
