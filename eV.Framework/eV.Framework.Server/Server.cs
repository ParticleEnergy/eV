// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Framework.Server.Logger;
using eV.Framework.Server.Options;
using eV.Framework.Server.SystemHandler;
using eV.Framework.Server.Utils;
using eV.Module.Cluster;
using eV.Module.GameProfile;
using eV.Module.Queue;
using eV.Module.Routing;
using eV.Module.Session;
using eV.Module.Storage.Mongo;
using eV.Module.Storage.Redis;
using eV.Network.Core;
using eV.Network.Core.Interface;
using eV.Network.Tcp.Server;
using StackExchange.Redis;
using eVNetworkServer = eV.Network.Tcp.Server.Server;
using EasyLogger = eV.Module.EasyLog.Logger;

namespace eV.Framework.Server;

public class Server
{
    private readonly eVNetworkServer _server = new(GetServerSetting());

    private readonly IdleDetection _idleDetection = new(Configure.Instance.ServerOption.SessionMaximumIdleTime);
    private readonly SessionExtension _sessionExtension = new();

    private Cluster? _cluster;
    private Queue? _queue;

    private readonly string _nodeId = Guid.NewGuid().ToString();

    public Server()
    {
        EasyLogger.SetLogger(new ServerLog(Configure.Instance.ProjectName));
        EasyLogger.Info(DefaultSetting.Logo);

        _server.AcceptConnect += ServerOnAcceptConnect;

        _sessionExtension.OnActivateEvent += ServerEvent.SessionOnActivate;
        _sessionExtension.OnReleaseEvent += ServerEvent.SessionOnRelease;

        if (Configure.Instance.BaseOption.Debug)
            SessionDebug.EnableDebug();

        Profile.Init(
            Configure.Instance.BaseOption.PublicObjectAssemblyString,
            Configure.Instance.BaseOption.GameProfilePath,
            Configure.Instance.BaseOption.GameProfileMonitoringChange
        );
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
        Dispatch.RegisterServer(
            Configure.Instance.BaseOption.ProjectAssemblyString,
            Configure.Instance.BaseOption.PublicObjectAssemblyString
        );

        Dispatch.AddCustomHandler(typeof(ClientKeepaliveHandler), typeof(ClientKeepalive));
        Dispatch.AddCustomHandler(typeof(ClientSendBroadcastHandler), typeof(ClientSendBroadcast));
        Dispatch.AddCustomHandler(typeof(ClientSendBySessionIdHandler), typeof(ClientSendBySessionId));
    }

    public async void Start()
    {
        // Mongodb
        if (Configure.Instance.MongodbOption != null)
            MongodbManager.Instance.Start(Configure.Instance.MongodbOption);

        // Redis
        if (Configure.Instance.RedisOption == null)
            return;
        Dictionary<string, ConfigurationOptions> configs = new();

        foreach ((string name, RedisOption option) in Configure.Instance.RedisOption)
            configs[name] = ConfigUtils.GetRedisConfig(option);

        await RedisManager.Instance.Start(configs);

        // Queue
        var queueRedis = RedisManager.Instance.GetRedisConnection(RedisNameReservedWord.QueueInstance);
        if (queueRedis != null)
        {
            _queue = new Queue(Configure.Instance.ProjectName, _nodeId, Configure.Instance.BaseOption.ProjectAssemblyString, queueRedis);
            _queue.Start();
        }

        // cluster
        var clusterRedis = RedisManager.Instance.GetRedisConnection(RedisNameReservedWord.ClusterInstance);
        if (clusterRedis != null)
        {
            _cluster = new Cluster(
                Configure.Instance.ProjectName,
                _nodeId,
                Configure.Instance.BaseOption.ProjectAssemblyString,
                clusterRedis,
                new CommunicationSetting
                {
                    SendAction = (sessionId, data) =>
                    {
                        Session? session = SessionDispatch.Instance.SessionManager.GetActiveSession(sessionId);
                        return session != null && session.Send(data);
                    },
                    SendBroadcastAction = (data) =>
                    {
                        if (SessionDispatch.Instance.SessionManager.GetActiveCount() <= 0)
                            return;

                        foreach ((string _, Session? session) in SessionDispatch.Instance.SessionManager.GetAllActiveSession())
                        {
                            if (session.SessionId == null)
                                continue;
                            session.Send(data);
                        }
                    },
                    SendBatchProcessingQuantity = Configure.Instance.ClusterOption == null ? 1 : Configure.Instance.ClusterOption.SendBatchProcessingQuantity,
                    SendBroadcastBatchProcessingQuantity = Configure.Instance.ClusterOption == null ? 1 : Configure.Instance.ClusterOption.SendBroadcastBatchProcessingQuantity
                });
            _cluster.Start();
        }

        RegisterHandler();

        ServerEvent.OnStart();
        _server.Start();

        _idleDetection.Start();
    }

    public void Stop()
    {
        _idleDetection.Stop();

        ServerEvent.OnStop();
        _server.Stop();

        _cluster?.Stop();
        _queue?.Stop();
        RedisManager.Instance.Stop();
    }
}
