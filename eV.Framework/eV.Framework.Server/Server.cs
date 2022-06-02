// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Framework.Server.SystemHandler;
using eV.Module.EasyLog;
using eV.Module.GameProfile;
using eV.Module.Queue;
using eV.Module.Routing;
using eV.Module.Routing.Interface;
using eV.Module.Session;
using eV.Network.Core;
using eV.Network.Core.Interface;
using eV.Network.Tcp.Server;
using eVNetworkServer = eV.Network.Tcp.Server.Server;
namespace eV.Framework.Server;

public class Server
{
    private readonly IdleDetection _idleDetection = new(Configure.Instance.ServerOption.SessionMaximumIdleTime);
    private readonly eVNetworkServer _server = new(GetServerSetting());
    private readonly Queue _queue = new(Configure.Instance.BaseOption.HandlerNamespace);

    private readonly SessionExtension _sessionExtension = new();

    public Server()
    {
        Logger.SetLogger(new Log(Configure.Instance.ProjectName));

        _sessionExtension.OnActivateEvent += SessionOnActivate;
        _sessionExtension.OnReleaseEvent += SessionOnRelease;

        _server.AcceptConnect += ServerOnAcceptConnect;
    }

    public void Start()
    {
        Logger.Info(DefaultSetting.Logo);
        Profile.Init(
            Configure.Instance.BaseOption.PublicObjectNamespace,
            Configure.Instance.BaseOption.GameProfilePath,
            new GameProfileParser(),
            Configure.Instance.BaseOption.GameProfileMonitoringChange
        );

        LoadModule.Run();
        RegisterHandler();

        _queue.Start();
        _server.Start();
        _idleDetection.Start();
    }

    public void Stop()
    {
        _idleDetection.Stop();
        _server.Stop();
        LoadModule.Stop();
    }

    private void ServerOnAcceptConnect(ITcpChannel channel)
    {
        if (_server.ServerState != RunState.On)
            return;
        Session session = SessionDispatch.Instance.SessionManager.GetSession(channel, _sessionExtension);
        OnConnected?.Invoke(session);
    }

    private static ServerSetting GetServerSetting()
    {
        ServerSetting serverSetting = new();
        if (!Configure.Instance.ServerOption.Host.Equals(""))
            serverSetting.Host = Configure.Instance.ServerOption.Host;

        if (Configure.Instance.ServerOption.Port > 0)
            serverSetting.Port = Configure.Instance.ServerOption.Port;

        if (Configure.Instance.ServerOption.Backlog > 0)
            serverSetting.Backlog = Configure.Instance.ServerOption.Backlog;

        if (Configure.Instance.ServerOption.MaxConnectionCount > 0)
            serverSetting.MaxConnectionCount = Configure.Instance.ServerOption.MaxConnectionCount;

        if (Configure.Instance.ServerOption.ReceiveBufferSize > 0)
            serverSetting.ReceiveBufferSize = Configure.Instance.ServerOption.ReceiveBufferSize;

        if (Configure.Instance.ServerOption.TcpKeepAliveTime > 0)
            serverSetting.TcpKeepAliveTime = Configure.Instance.ServerOption.TcpKeepAliveTime;

        if (Configure.Instance.ServerOption.TcpKeepAliveInterval > 0)
            serverSetting.TcpKeepAliveInterval = Configure.Instance.ServerOption.TcpKeepAliveInterval;

        if (Configure.Instance.ServerOption.TcpKeepAliveRetryCount > 0)
            serverSetting.TcpKeepAliveRetryCount = Configure.Instance.ServerOption.TcpKeepAliveRetryCount;

        return serverSetting;
    }

    private static void RegisterHandler()
    {
        Dispatch.RegisterServer(Configure.Instance.BaseOption.HandlerNamespace, Configure.Instance.BaseOption.PublicObjectNamespace);

        Dispatch.AddCustomHandler(typeof(ClientKeepaliveHandler), typeof(ClientKeepalive));
        Dispatch.AddCustomHandler(typeof(ClientJoinGroupHandler), typeof(ClientJoinGroup));
        Dispatch.AddCustomHandler(typeof(ClientLeaveGroupHandler), typeof(ClientLeaveGroup));
        Dispatch.AddCustomHandler(typeof(ClientSendBroadcastHandler), typeof(ClientSendBroadcast));
        Dispatch.AddCustomHandler(typeof(ClientSendBySessionIdHandler), typeof(ClientSendBySessionId));
        Dispatch.AddCustomHandler(typeof(ClientSendGroupHandler), typeof(ClientSendGroup));
    }
    #region Event
    public event SessionEvent? OnConnected;
    public event SessionEvent? SessionOnActivate;
    public event SessionEvent? SessionOnRelease;
    #endregion
}
