// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.Routing;
namespace eV.Unity;

public class SessionDispatch
{

    private Session.Session? _session;
    public static SessionDispatch Instance { get; } = new();

    public void SetClientSession(Session.Session session)
    {
        _session = session;
        Extension();

        ClientSession.Init(session);
        ClientSession.Activate();
    }

    private void Extension()
    {
        _session!.SendAction = (sessionId, data) =>
        {
            Packet packet = new();
            packet.SetName("ClientSendBySessionId");
            packet.SetContent(Serializer.Serialize(new
            {
                ClientSendBySessionId = sessionId, Data = data
            }));
            return _session.Send(Package.Pack(packet));
        };

        _session.SendGroupAction = (_, groupId, data) =>
        {
            Packet packet = new();
            packet.SetName("ClientSendGroup");
            packet.SetContent(Serializer.Serialize(new
            {
                GroupId = groupId, Data = data
            }));
            _session.Send(Package.Pack(packet));
        };

        _session.SendBroadcastAction = (_, data) =>
        {
            Packet packet = new();
            packet.SetName("ClientSendBroadcast");
            packet.SetContent(Serializer.Serialize(new
            {
                Data = data
            }));
            _session.Send(Package.Pack(packet));
        };

        _session.JoinGroupAction = (groupId, sessionId) =>
        {
            Packet packet = new();
            packet.SetName("ClientJoinGroup");
            packet.SetContent(Serializer.Serialize(new
            {
                GroupId = groupId, SessionId = sessionId
            }));
            return _session.Send(Package.Pack(packet));
        };

        _session.LeaveGroupAction = (groupId, sessionId) =>
        {
            Packet packet = new();
            packet.SetName("ClientLeaveGroup");
            packet.SetContent(Serializer.Serialize(new
            {
                GroupId = groupId, SessionId = sessionId
            }));
            return _session.Send(Package.Pack(packet));
        };
    }
}
