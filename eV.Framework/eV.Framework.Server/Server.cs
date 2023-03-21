// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using System.Net;
using System.Security.Cryptography.X509Certificates;
using eV.Framework.Server.ClusterCommunication;
using eV.Framework.Server.Logger;
using eV.Framework.Server.Object;
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
    private const string Logo = @"
      _    __   _____
  ___| |  / /  / ___/___  ______   _____  _____
 / _ \ | / /   \__ \/ _ \/ ___/ | / / _ \/ ___/
/  __/ |/ /   ___/ /  __/ /   | |/ /  __/ /
\___/|___/   /____/\___/_/    |___/\___/_/
";

    private readonly eVNetworkServer _server = new(GetServerSetting());

    private readonly IdleDetection _idleDetection = new(Configure.Instance.ServerOption.SessionMaximumIdleTime);
    private readonly SessionExtension _sessionExtension = new();

    private Cluster? _cluster;
    private Queue? _queue;

    private readonly string _nodeId = Guid.NewGuid().ToString();

    public Server()
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

        EasyLogger.SetLogger(new ServerLog(Configure.Instance.ProjectName));
        EasyLogger.Info(Logo);
        EasyLogger.Info($"NodeId [{_nodeId}]");
        _server.AcceptConnect += ServerOnAcceptConnect;

        _sessionExtension.OnActivateEvent += ServerEvent.SessionOnActivate;
        _sessionExtension.OnReleaseEvent += ServerEvent.SessionOnRelease;

        Profile.Init(
            Configure.Instance.BaseOption.PublicObjectAssemblyString,
            Configure.Instance.BaseOption.GameProfilePath
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
        ServerSetting serverSetting = new() { TcpKeepAlive = Configure.Instance.ServerOption.TcpKeepAlive };

        if (Configure.Instance.ServerOption.TlsCertFile != string.Empty)
        {
            X509Certificate2 x509Certificate = new(Configure.Instance.ServerOption.TlsCertFile, Configure.Instance.ServerOption.TlsCertPassword);

            serverSetting.TlsX509Certificate2 = x509Certificate;
            serverSetting.TlsClientCertificateRequired = Configure.Instance.ServerOption.TlsClientCertificateRequired;
            serverSetting.TlsCheckCertificateRevocation = Configure.Instance.ServerOption.TlsCheckCertificateRevocation;
        }

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

        return serverSetting;
    }

    private static void RegisterHandler()
    {
        Dispatch.RegisterServer(
            Configure.Instance.BaseOption.ProjectAssemblyString,
            Configure.Instance.BaseOption.PublicObjectAssemblyString
        );

        Dispatch.AddCustomHandler(typeof(ClientSendBroadcastHandler), typeof(ClientSendBroadcastPackage));
        Dispatch.AddCustomHandler(typeof(ClientSendBySessionIdHandler), typeof(SendBySessionIdPackage));
    }

    public void Start()
    {
        // Mongodb
        if (Configure.Instance.MongodbOption != null)
            MongodbManager.Instance.Start(Configure.Instance.MongodbOption);

        // Redis
        if (Configure.Instance.RedisOption != null)
        {
            Dictionary<string, ConfigurationOptions> configs = new();

            foreach ((string name, RedisOption option) in Configure.Instance.RedisOption)
                configs[name] = ConfigUtils.GetRedisConfig(option);

            RedisManager.Instance.Start(configs);
        }

        // Queue
        var queueRedis = RedisManager.Instance.GetRedisConnection(RedisNameReservedWord.QueueInstance);
        if (queueRedis != null)
        {
            _queue = new Queue(Configure.Instance.ProjectName, _nodeId, Configure.Instance.BaseOption.ProjectAssemblyString, queueRedis);
        }

        // cluster
        var clusterRedis = RedisManager.Instance.GetRedisConnection(RedisNameReservedWord.ClusterInstance);
        if (clusterRedis != null)
        {
            _cluster = new Cluster(
                Configure.Instance.ProjectName,
                _nodeId,
                Configure.Instance.BaseOption.ProjectAssemblyString,
                clusterRedis
            );

            _cluster.AddCustomHandler(typeof(SendBroadcastInternalHandler), typeof(InternalSendBroadcastPackage));
            _cluster.AddCustomHandler(typeof(SendInternalHandler), typeof(SendBySessionIdPackage));
        }

        RegisterHandler();

        _queue?.Start();
        _cluster?.Start();

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
