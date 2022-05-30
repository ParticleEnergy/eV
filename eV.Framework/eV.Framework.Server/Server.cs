// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Framework.Server.SystemHandler;
using eV.Module.EasyLog;
using eV.Module.GameProfile;
using eV.Module.Routing;
using eV.Module.Routing.Interface;
using eV.Module.Session;
using eV.Module.Storage.Mongo;
using eV.Module.Storage.Redis;
using eV.Network.Core;
using eV.Network.Core.Interface;
using eV.Network.Tcp.Server;
using StackExchange.Redis;
using eVNetworkServer = eV.Network.Tcp.Server.Server;
namespace eV.Framework.Server;

public class Server
{
    private readonly IdleDetection _idleDetection = new(Configure.Instance.ServerOption.SessionMaximumIdleTime);
    private readonly eVNetworkServer _server = new(GetServerSetting());

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
        LoadMongodb();
        LoadRedis();
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


    #region Storage
    private static void LoadRedis()
    {
        if (Configure.Instance.RedisOption == null)
            return;
        Dictionary<string, ConfigurationOptions> configs = new();

        foreach ((string name, var option) in Configure.Instance.RedisOption)
        {
            if (option.Address.Length <= 0)
                return;

            ConfigurationOptions config = new();
            foreach (string[] address in option.Address.Select(address => address.Split(":")))
            {
                config.EndPoints.Add(address[0] , Convert.ToInt32(address[1]));
            }

            if (option.User != null)
                config.User = option.User;
            if (option.Password != null)
                config.Password = option.Password;
            if (option.Keepalive != null)
                config.KeepAlive = (int)option.Keepalive;
            config.DefaultVersion = new Version(4, 0, 9);
            config.DefaultDatabase = option.Address.Length == 1 && option.Database != null ? option.Database : 0;

            configs[name] = config;
        }
        RedisManager.Instance.Start(configs);
    }

    private static void LoadMongodb()
    {
        if (Configure.Instance.MongodbOption != null)
            MongodbManager.Instance.Start(Configure.Instance.MongodbOption);
    }
    #endregion
}
