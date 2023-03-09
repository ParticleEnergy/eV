// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using System.Net;
using System.Security.Cryptography.X509Certificates;
using eV.Module.EasyLog;
using eV.Module.Routing;
using eV.Module.Routing.Interface;
using eV.Module.Session;
using eV.Network.Core.Interface;
using eV.Network.Tcp.Client;
using eVNetworkClient = eV.Network.Tcp.Client.Client;

namespace eV.Framework.Unity;

public class Client
{
    private const string Logo = @"
      _    __   __  __      _ __
  ___| |  / /  / / / /___  (_) /___  __
 / _ \ | / /  / / / / __ \/ / __/ / / /
/  __/ |/ /  / /_/ / / / / / /_/ /_/ /
\___/|___/   \____/_/ /_/_/\__/\__, /
                              /____/
";

    private readonly ITcpClient _client;

    public Client(UnitySetting setting)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        if (setting.Log != null)
        {
            Logger.SetLogger(setting.Log);
            Logger.Info(Logo);
        }

        ClientSetting clientSetting = new() { Host = setting.Host, Port = setting.Port, ReceiveBufferSize = setting.ReceiveBufferSize, TcpKeepAlive = setting.TcpKeepAlive };

        if (setting is { TlsCertData: { }, TlsTargetHost: { } })
        {
            X509CertificateCollection x509CertificateCollection = new();
            X509Certificate x509Certificate = new X509Certificate2(setting.TlsCertData, setting.TlsCertPassword);
            x509CertificateCollection.Add(x509Certificate);

            clientSetting.TlsTargetHost = setting.TlsTargetHost;
            clientSetting.TlsX509CertificateCollection = x509CertificateCollection;
            clientSetting.TlsCheckCertificateRevocation = setting.TlsCheckCertificateRevocation;
        }

        _client = new eVNetworkClient(clientSetting);

        _client.ConnectCompleted += ClientOnConnectCompleted;
        _client.DisconnectCompleted += ClientOnDisconnectCompleted;

        Dispatch.RegisterClient(setting.ProjectAssemblyString, setting.PublicObjectAssemblyString);
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
    }

    private void ClientOnDisconnectCompleted(ITcpChannel _)
    {
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
            packet.SetName("SendBySessionIdPackage");
            packet.SetContent(Serializer.Serialize(new { SessionId = sessionId, Data = data }));
            return Task.FromResult(session.Send(Package.Pack(packet)));
        };

        session.SendBroadcastAction = (_, data) =>
        {
            Packet packet = new();
            packet.SetName("ClientSendBroadcastPackage");
            packet.SetContent(Serializer.Serialize(new { Data = data }));
            session.Send(Package.Pack(packet));
        };
    }

    #endregion
}
