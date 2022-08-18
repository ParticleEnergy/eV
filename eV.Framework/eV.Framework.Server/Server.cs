// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


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
namespace eV.Framework.Server;

public class Server
{
    private readonly IdleDetection _idleDetection = new(Configure.Instance.TcpServerOption.SessionMaximumIdleTime);
    private readonly Queue _queue = new(Configure.Instance.BaseOption.ProjectAssemblyString);
    private readonly eVNetworkServer _server = new(GetServerSetting());

    private readonly SessionExtension _sessionExtension = new();

    private Cluster? _cluster;

    public Server()
    {
        _server.AcceptConnect += ServerOnAcceptConnect;

        _sessionExtension.OnActivateEvent += ServerEvent.SessionOnActivate;
        _sessionExtension.OnReleaseEvent += ServerEvent.SessionOnRelease;

        InitServerSession();
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
                ConsumeSendBroadcastPipelineNumber = Configure.Instance.ClusterOption.ConsumeSendBroadcastPipelineNumber,
                RedisOption = ConfigUtils.GetRedisConfig(Configure.Instance.ClusterOption.Redis),
                KafkaOption = ConfigUtils.GetKafkaConfig(Configure.Instance.ClusterOption.Kafka),
                SendAction = SessionUtils.SendAction,
                SendGroupAction = SessionUtils.SendGroupAction,
                SendBroadcastAction = SessionUtils.SendBroadcastAction
            });

            ServerSession.SetSessionDrive(new ClusterSessionDrive(_cluster.GetClusterSession()));
        }
    }

    public void Start()
    {
        LoadModule.Start();

        Profile.Init(
            Configure.Instance.BaseOption.PublicObjectAssemblyString,
            Configure.Instance.BaseOption.GameProfilePath,
            Configure.Instance.BaseOption.GameProfileMonitoringChange
        );

        RegisterHandler();

        _cluster?.Start();
        _queue.Start();
        _server.Start();
        _idleDetection.Start();
    }

    public void Stop()
    {
        _idleDetection.Stop();
        _cluster?.Stop();
        _server.Stop();
        LoadModule.Stop();
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
        if (!Configure.Instance.TcpServerOption.Host.Equals(""))
            serverSetting.Host = Configure.Instance.TcpServerOption.Host;

        if (Configure.Instance.TcpServerOption.Port > 0)
            serverSetting.Port = Configure.Instance.TcpServerOption.Port;

        if (Configure.Instance.TcpServerOption.Backlog > 0)
            serverSetting.Backlog = Configure.Instance.TcpServerOption.Backlog;

        if (Configure.Instance.TcpServerOption.MaxConnectionCount > 0)
            serverSetting.MaxConnectionCount = Configure.Instance.TcpServerOption.MaxConnectionCount;

        if (Configure.Instance.TcpServerOption.ReceiveBufferSize > 0)
            serverSetting.ReceiveBufferSize = Configure.Instance.TcpServerOption.ReceiveBufferSize;

        if (Configure.Instance.TcpServerOption.TcpKeepAliveTime > 0)
            serverSetting.TcpKeepAliveTime = Configure.Instance.TcpServerOption.TcpKeepAliveTime;

        if (Configure.Instance.TcpServerOption.TcpKeepAliveInterval > 0)
            serverSetting.TcpKeepAliveInterval = Configure.Instance.TcpServerOption.TcpKeepAliveInterval;

        if (Configure.Instance.TcpServerOption.TcpKeepAliveRetryCount > 0)
            serverSetting.TcpKeepAliveRetryCount = Configure.Instance.TcpServerOption.TcpKeepAliveRetryCount;

        return serverSetting;
    }

    private static void RegisterHandler()
    {
        Dispatch.RegisterServer(Configure.Instance.BaseOption.ProjectAssemblyString, Configure.Instance.BaseOption.PublicObjectAssemblyString);

        Dispatch.AddCustomHandler(typeof(ClientKeepaliveHandler), typeof(ClientKeepalive));
        Dispatch.AddCustomHandler(typeof(ClientJoinGroupHandler), typeof(ClientJoinGroup));
        Dispatch.AddCustomHandler(typeof(ClientLeaveGroupHandler), typeof(ClientLeaveGroup));
        Dispatch.AddCustomHandler(typeof(ClientSendBroadcastHandler), typeof(ClientSendBroadcast));
        Dispatch.AddCustomHandler(typeof(ClientSendBySessionIdHandler), typeof(ClientSendBySessionId));
        Dispatch.AddCustomHandler(typeof(ClientSendGroupHandler), typeof(ClientSendGroup));
    }
}
