// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.


using eV.EasyLog;
using eV.GameProfile;
using eV.Network.Core.Interface;
using eV.Routing;
using eV.Routing.Interface;
using eVNetworkClient = eV.Network.Tcp.Client.Client;
using eVNetworkSecurityClient = eV.Network.Tcp.Security.Client.Client;

namespace eV.Client;

public class Client
{
    private readonly ITcpClient _client;

    public Client(ClientSetting setting)
    {
        Logger.SetLogger(new Log());
        Logger.Info(DefaultSetting.Logo);

        if (setting.CertFile.Equals(""))
        {
            Network.Tcp.Client.ClientSetting clientSetting = new()
            {
                Host = setting.Host,
                Port = setting.Port,
                ReceiveBufferSize = setting.ReceiveBufferSize
            };
            _client = new eVNetworkClient(clientSetting);
        }
        else
        {
            Network.Tcp.Security.Client.ClientSetting clientSetting = new()
            {
                Host = setting.Host,
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
    }
    public void Connect()
    {
        _client.Connect();
    }

    public void Disconnect()
    {
        _client.Disconnect();
    }

    private void ClientOnConnectCompleted(ITcpChannel channel)
    {
        Session.Session session = new(channel);
        SessionDispatch.Instance.SetClientSession(session);
        OnConnect?.Invoke(session);
    }
    private void ClientOnDisconnectCompleted(ITcpChannel _)
    {
        OnDisconnect?.Invoke();
    }
    #region event
    public event SessionEvent? OnConnect;
    public event Action? OnDisconnect;
    #endregion
}
