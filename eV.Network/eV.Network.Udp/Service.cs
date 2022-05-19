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
    public RunState ServiceState
    {
        get;
        private set;
    }
    #endregion

    #region Setting
    private IPEndPoint? _listenEndPoint;
    private IPEndPoint? _multiCastEndPoint;
    private IPEndPoint? _broadcastEndPoint;
    private MulticastOption? _multicastOption;
    private int _multicastTimeToLive;
    private bool _multicastLoopback;
    private int _receiveBufferSize;
    private int _maxConcurrentSend;
    #endregion

    #region Resource
    private readonly Socket _socket;
    private readonly UdpChannel _channel;
    #endregion

    #region Event
    public event UdpChannelEvent? OnBind;
    public event UdpChannelEvent? OnRelease;
    #endregion

    #region Construct
    public Service(ServiceSetting setting)
    {
        SetSetting(setting);
        ServiceState = RunState.Off;

        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // 广播
        _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
        //设置多播
        _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, _multicastLoopback);
        _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, _multicastOption!);
        _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, _multicastTimeToLive);

        _channel = new UdpChannel(_receiveBufferSize, _broadcastEndPoint!, _multiCastEndPoint!);
        _channel.OpenCompleted += OpenCompleted;
        _channel.CloseCompleted += CloseCompleted;
    }

    private void SetSetting(ServiceSetting setting)
    {
        _broadcastEndPoint = new IPEndPoint(
            IPAddress.Broadcast,
            setting.CommPort
        );
        _multiCastEndPoint = new IPEndPoint(
            IPAddress.Parse(setting.MultiCastHost),
            setting.CommPort
        );
        _listenEndPoint = new IPEndPoint(
            IPAddress.Any,
            setting.ListenPort
        );
        _multicastOption = new MulticastOption(IPAddress.Parse(setting.MultiCastHost), IPAddress.Parse(setting.Localhost));
        _multicastTimeToLive = setting.MulticastTimeToLive;
        _multicastLoopback  =setting.MulticastLoopback;
        _receiveBufferSize = setting.ReceiveBufferSize;
        _maxConcurrentSend = setting.MaxConcurrentSend;
    }
    #endregion

    #region Operate
    public void Start()
    {
        if (ServiceState == RunState.On)
        {
            Logger.Warn("The server is already turned on");
            return;
        }
        try
        {
            ServiceState = RunState.On;

            if (_listenEndPoint!.AddressFamily == AddressFamily.InterNetworkV6)
            {
                _socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
                _socket.Bind(_listenEndPoint);
            }
            else
            {
                _socket.Bind(_listenEndPoint);
            }

            _channel.Open(_socket, _maxConcurrentSend);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }
    public void Stop()
    {
        if (ServiceState == RunState.Off)
        {
            Logger.Warn("The server is already turned off");
            return;
        }
        ServiceState = RunState.Off;
        try
        {
            _socket.Shutdown(SocketShutdown.Both);
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
        _channel.Close();
    }
    #endregion

    #region Channel
    private void OpenCompleted(IUdpChannel channel)
    {
        OnBind?.Invoke(channel);
    }
    private void CloseCompleted(IUdpChannel channel)
    {
        OnRelease?.Invoke(channel);
    }
    #endregion
}
