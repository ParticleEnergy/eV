// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Module.EasyLog;
using eV.Module.GameProfile;
using eV.Module.Routing;
using eV.Module.Routing.Interface;
using eV.Module.Session;
using eV.Network.Core.Interface;
using eV.Network.Tcp.Client;
using eVNetworkClient = eV.Network.Tcp.Client.Client;
using eVNetworkSecurityClient = eV.Network.Tcp.Security.Client.Client;

namespace eV.Framework.Unity;

public class Client
{
    private readonly ITcpClient _client;

    private readonly Keepalive _keepalive;
    public Client(UnitySetting setting)
    {
        Logger.SetLogger(new Log());
        Logger.Info(DefaultSetting.Logo);

        if (setting.CertFile.Equals(""))
        {
            ClientSetting clientSetting = new()
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
            setting.PublicObjectAssemblyString,
            setting.GameProfilePath,
            new GameProfileParser()
        );

        Dispatch.RegisterClient(setting.HandlerAssemblyString, setting.PublicObjectAssemblyString);
        _keepalive = new Keepalive(setting.TcpKeepAliveInterval);
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
        Session session = new(channel);
        ExtensionSession(session);
        OnConnect?.Invoke(session);
        _keepalive.Start(session);
    }
    private void ClientOnDisconnectCompleted(ITcpChannel _)
    {
        _keepalive.Stop();
        OnDisconnect?.Invoke();
    }
    #region event
    public event SessionEvent? OnConnect;
    public event Action? OnDisconnect;
    #endregion


    #region Session
    private static void ExtensionSession(Session session)
    {
        session.SendAction = (sessionId, data) =>
        {
            Packet packet = new();
            packet.SetName("ClientSendBySessionId");
            packet.SetContent(Serializer.Serialize(new
            {
                ClientSendBySessionId = sessionId, Data = data
            }));
            return session.Send(Package.Pack(packet));
        };

        session.SendGroupAction = (_, groupId, data) =>
        {
            Packet packet = new();
            packet.SetName("ClientSendGroup");
            packet.SetContent(Serializer.Serialize(new
            {
                GroupId = groupId, Data = data
            }));
            session.Send(Package.Pack(packet));
        };

        session.SendBroadcastAction = (_, data) =>
        {
            Packet packet = new();
            packet.SetName("ClientSendBroadcast");
            packet.SetContent(Serializer.Serialize(new
            {
                Data = data
            }));
            session.Send(Package.Pack(packet));
        };

        session.JoinGroupAction = (groupId, sessionId) =>
        {
            Packet packet = new();
            packet.SetName("ClientJoinGroup");
            packet.SetContent(Serializer.Serialize(new
            {
                GroupId = groupId, SessionId = sessionId
            }));
            return session.Send(Package.Pack(packet));
        };

        session.LeaveGroupAction = (groupId, sessionId) =>
        {
            Packet packet = new();
            packet.SetName("ClientLeaveGroup");
            packet.SetContent(Serializer.Serialize(new
            {
                GroupId = groupId, SessionId = sessionId
            }));
            return session.Send(Package.Pack(packet));
        };
    }
    #endregion
}
