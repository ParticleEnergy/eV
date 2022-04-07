// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.


using eV.EasyLog;
using eV.GameProfile;
using eV.Network.Client;
using eV.Network.Core.Interface;
using eV.Routing;
using eV.Routing.Interface;
using eVNetworkClient = eV.Network.Client.Client;
using eVNetworkSecurityClient = eV.Network.Security.Client.Client;

namespace eV.Unity;

public class Client
{
    private readonly IClient _client;

    private readonly Keepalive _keepalive;
    public Client(UnitySetting setting)
    {
        Logger.SetLogger(new Log());
        Logger.Info(DefaultSetting.Logo);

        if (setting.CertFile.Equals(""))
        {
            ClientSetting clientSetting = new()
            {
                Address = setting.Host,
                Port = setting.Port,
                ReceiveBufferSize = setting.ReceiveBufferSize
            };
            _client = new eVNetworkClient(clientSetting);
        }
        else
        {
            Network.Security.Client.ClientSetting clientSetting = new()
            {
                Address = setting.Host,
                Port = setting.Port,
                ReceiveBufferSize = setting.ReceiveBufferSize,
                TargetHost = setting.TargetHost,
                CertFile = setting.CertFile,
                SslProtocols = setting.SslProtocols
            };
            _client = new eVNetworkSecurityClient(clientSetting);
        }

        _client.ConnectCompleted += ClientOnConnectCompleted;
        _client.DisconnectCompleted += ClientOnDisconnectCompleted;

        Profile.Init(
            setting.DataStructNamespace,
            setting.GameProfilePath,
            new GameProfileParser()
        );

        Dispatch.RegisterClient(setting.HandlerNamespace, setting.DataStructNamespace);
        _keepalive = new Keepalive(setting.TcpKeepAliveInterval);
    }
    public void Start()
    {
        _client.Connect();
    }

    private void ClientOnConnectCompleted(IChannel channel)
    {
        Session.Session session = new(channel);
        SessionDispatch.Instance.SetClientSession(session);
        OnConnect?.Invoke(session);
        _keepalive.Start();
    }
    private void ClientOnDisconnectCompleted(IChannel _)
    {
        _keepalive.Stop();
        OnDisconnect?.Invoke();
    }
    #region event
    public event SessionEvent? OnConnect;
    public event Action? OnDisconnect;
    #endregion
}
