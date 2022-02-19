// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.EasyLog;
using eV.Routing.Interface;
using eV.Session.Interface;
namespace eV.Server;

public class SessionExtension : ISessionExtend
{

    public bool Send(string sessionId, byte[] data)
    {
        return ServerSession.Send(sessionId, data);
    }
    public void SendGroup(string selfSessionId, string groupId, byte[] data)
    {
        ServerSession.SendGroup(selfSessionId, groupId, data);
    }
    public void SendBroadcast(string selfSessionId, byte[] data)
    {
        ServerSession.SendBroadcast(selfSessionId, data);
    }
    public bool JoinGroup(string groupId, string sessionId)
    {
        return ServerSession.JoinGroup(groupId, sessionId);
    }
    public bool LeaveGroup(string groupId, string sessionId)
    {
        return ServerSession.LeaveGroup(groupId, sessionId);
    }
    public void OnActivate(ISession session)
    {
        if (ServerSession.Activate(session))
        {
            OnActivateEvent?.Invoke(session);
        }
        else
        {
            Logger.Error($"Session {session.SessionId} Session add active group error");
            session.Shutdown();
        }
    }
    public void OnRelease(ISession session)
    {
        if (!ServerSession.Release(session) && session.SessionId is null or "")
            Logger.Error($"Session {session.SessionId} Session remove active group error");
        OnReleaseEvent?.Invoke(session);
    }
    public event SessionEvent? OnActivateEvent;
    public event SessionEvent? OnReleaseEvent;
}
