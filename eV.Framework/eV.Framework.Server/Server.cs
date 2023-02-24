// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Framework.Server.Logger;
using eV.Framework.Server.SessionDrive;
using eV.Framework.Server.SystemHandler;
using eV.Framework.Server.Utils;
using eV.Module.Cluster;
using eV.Module.GameProfile;
using eV.Module.Queue;
using eV.Module.Routing;
using eV.Module.Session;
using eV.Network.Core;
using eV.Network.Core.Interface;
using eV.Network.Tcp.Server;
using eVNetworkServer = eV.Network.Tcp.Server.Server;
using EasyLogger = eV.Module.EasyLog.Logger;

namespace eV.Framework.Server;

public class Server
{
    private readonly IdleDetection _idleDetection = new(Configure.Instance.ServerOption.SessionMaximumIdleTime);
    private readonly Queue _queue = new(Configure.Instance.BaseOption.ProjectAssemblyString);
    private readonly eVNetworkServer _server = new(GetServerSetting());

    private readonly SessionExtension _sessionExtension = new();

    private Cluster? _cluster;

    public Server()
    {
        EasyLogger.SetLogger(new ServerLog(Configure.Instance.ProjectName));
        EasyLogger.Info(DefaultSetting.Logo);

        _server.AcceptConnect += ServerOnAcceptConnect;

        _sessionExtension.OnActivateEvent += ServerEvent.SessionOnActivate;
        _sessionExtension.OnReleaseEvent += ServerEvent.SessionOnRelease;

        InitServerSession();

        if (Configure.Instance.BaseOption.Debug)
            SessionDebug.EnableDebug();

        LoadModule.StartAll();

        Profile.Init(
            Configure.Instance.BaseOption.PublicObjectAssemblyString,
            Configure.Instance.BaseOption.GameProfilePath,
            Configure.Instance.BaseOption.GameProfileMonitoringChange
        );

        RegisterHandler();
    }

    private void InitServerSession()
    {
        if (Configure.Instance.ClusterOption == null)
        {
            ServerSession.SetSessionDrive(new SingleSessionDrive());
        }
        else
        {
            _cluster = new Cluster(new ClusterSetting
            {
                ClusterName = Configure.Instance.ProjectName,
                ConsumeSendPipelineNumber = Configure.Instance.ClusterOption.ConsumeSendPipelineNumber,
                ConsumeSendGroupPipelineNumber = Configure.Instance.ClusterOption.ConsumeSendGroupPipelineNumber,
                ConsumeSendBroadcastPipelineNumber =
                    Configure.Instance.ClusterOption.ConsumeSendBroadcastPipelineNumber,
                RedisOption = ConfigUtils.GetRedisConfig(Configure.Instance.ClusterOption.Redis),
                KafkaOption = ConfigUtils.GetKafkaConfig(Configure.Instance.ClusterOption.Kafka),
                SendAction = SessionUtils.SendAction,
                SendBroadcastAction = SessionUtils.SendBroadcastAction
            });

            ServerSession.SetSessionDrive(new ClusterSessionDrive(_cluster.GetClusterSession()));
        }
    }

    public void Start()
    {
        ServerEvent.OnStart();
        _cluster?.Start();
        _queue.Start();
        _server.Start();
        _idleDetection.Start();
    }

    public void Stop()
    {
        ServerEvent.OnStop();
        _idleDetection.Stop();
        _cluster?.Stop();
        _server.Stop();
        LoadModule.StopAll();
    }

    private void ServerOnAcceptConnect(ITcpChannel channel)
    {
        if (_server.ServerState != RunState.On)
            return;
        Session session = SessionDispatch.Instance.SessionManager.GetSession(channel, _sessionExtension);
        ServerEvent.OnConnected(session);
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
        Dispatch.RegisterServer(Configure.Instance.BaseOption.ProjectAssemblyString,
            Configure.Instance.BaseOption.PublicObjectAssemblyString);

        Dispatch.AddCustomHandler(typeof(ClientKeepaliveHandler), typeof(ClientKeepalive));
        Dispatch.AddCustomHandler(typeof(ClientSendBroadcastHandler), typeof(ClientSendBroadcast));
        Dispatch.AddCustomHandler(typeof(ClientSendBySessionIdHandler), typeof(ClientSendBySessionId));
    }
}
