// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.


using eV.EasyLog;
using eV.GameProfile;
using eV.Network.Core;
using eV.Network.Server;
using eV.Routing;
using eV.Routing.Interface;
using eV.Server.Storage;
using eV.Server.SystemHandler;
using eVNetworkServer = eV.Network.Server.Server;
namespace eV.Server;

public class Server
{
    private readonly IdleDetection _idleDetection = new(Configure.Instance.ServerOptions.SessionMaximumIdleTime);
    private readonly eVNetworkServer _server = new(GetServerSetting());

    private readonly SessionExtension _sessionExtension = new();

    public Server()
    {
        Logger.SetLogger(new Log());

        _sessionExtension.OnActivateEvent += SessionOnActivate;
        _sessionExtension.OnReleaseEvent += SessionOnRelease;

        _server.AcceptConnect += ServerOnAcceptConnect;
    }

    public void Start()
    {
        Logger.Info(DefaultSetting.Logo);
        Profile.Init(
            Configure.Instance.BaseOptions.ProjectNamespace,
            Configure.Instance.BaseOptions.GameProfilePath,
            new GameProfileParser(),
            Configure.Instance.BaseOptions.GameProfileMonitoringChange
        );
        MongodbManager.Instance.Start();
        RedisManager.Instance.Start();
        RegisterHandler();

        _server.Start();
        _idleDetection.Start();
    }

    public void Stop()
    {
        _idleDetection.Stop();
        _server.Stop();
        RedisManager.Instance.Stop();
    }

    private void ServerOnAcceptConnect(Channel channel)
    {
        if (_server.ServerState != RunState.On)
            return;
        Session.Session? session = SessionDispatch.Instance.SessionManager.GetSession(channel, _sessionExtension);
        OnConnected?.Invoke(session);
    }

    private static ServerSetting GetServerSetting()
    {
        ServerSetting serverSetting = new();
        if (!Configure.Instance.ServerOptions.Host.Equals(""))
            serverSetting.Address = Configure.Instance.ServerOptions.Host;

        if (!Configure.Instance.ServerOptions.Host.Equals(""))
            serverSetting.Address = Configure.Instance.ServerOptions.Host;

        if (Configure.Instance.ServerOptions.Port > 0)
            serverSetting.Port = Configure.Instance.ServerOptions.Port;

        if (Configure.Instance.ServerOptions.Backlog > 0)
            serverSetting.Backlog = Configure.Instance.ServerOptions.Backlog;

        if (Configure.Instance.ServerOptions.MaxConnectionCount > 0)
            serverSetting.MaxConnectionCount = Configure.Instance.ServerOptions.MaxConnectionCount;

        if (Configure.Instance.ServerOptions.ReceiveBufferSize > 0)
            serverSetting.ReceiveBufferSize = Configure.Instance.ServerOptions.ReceiveBufferSize;

        if (Configure.Instance.ServerOptions.TcpKeepAliveTime > 0)
            serverSetting.TcpKeepAliveTime = Configure.Instance.ServerOptions.TcpKeepAliveTime;

        if (Configure.Instance.ServerOptions.TcpKeepAliveInterval > 0)
            serverSetting.TcpKeepAliveInterval = Configure.Instance.ServerOptions.TcpKeepAliveInterval;

        if (Configure.Instance.ServerOptions.TcpKeepAliveRetryCount > 0)
            serverSetting.TcpKeepAliveRetryCount = Configure.Instance.ServerOptions.TcpKeepAliveRetryCount;

        return serverSetting;
    }

    private static void RegisterHandler()
    {
        Dispatch.Register(Configure.Instance.BaseOptions.ProjectNamespace);

        Dispatch.AddCustomHandler(typeof(ClientKeepaliveHandler), typeof(ClientKeepalive));
        Dispatch.AddCustomHandler(typeof(ClientJoinGroupHandler), typeof(ClientJoinGroup));
        Dispatch.AddCustomHandler(typeof(ClientLeaveGroupHandler), typeof(ClientLeaveGroup));
        Dispatch.AddCustomHandler(typeof(ClientSendBroadcastHandler), typeof(ClientSendBroadcast));
        Dispatch.AddCustomHandler(typeof(ClientSendBySessionIdHandler), typeof(ClientSendBySessionId));
        Dispatch.AddCustomHandler(typeof(ClientSendGroupHandler), typeof(ClientSendGroup));
    }
    #region Event
    public SessionEvent? OnConnected { private get; set; }
    public event SessionEvent? SessionOnActivate;
    public event SessionEvent? SessionOnRelease;
    #endregion
}
