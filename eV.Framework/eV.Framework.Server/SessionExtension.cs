// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using eV.Module.Routing.Interface;
using eV.Module.Session.Interface;
using EasyLogger = eV.Module.EasyLog.Logger;

namespace eV.Framework.Server;

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

    public ConcurrentDictionary<string, string>? GetGroup(string groupId)
    {
        return ServerSession.GetGroup(groupId);
    }

    public bool CreateGroup(string groupId)
    {
        return ServerSession.CreateGroup(groupId);
    }

    public bool DeleteGroup(string groupId)
    {
        return ServerSession.DeleteGroup(groupId);
    }

    public void OnActivate(ISession session)
    {
        if (ServerSession.Activate(session))
        {
            OnActivateEvent?.Invoke(session);
        }
        else
        {
            EasyLogger.Error($"Session {session.SessionId} Session add active group error");
            session.Shutdown();
        }
    }

    public void OnRelease(ISession session)
    {
        if (session.SessionId is not (null or ""))
        {
            if (!ServerSession.Release(session))
                EasyLogger.Warn($"Session {session.SessionId} Session remove active group error");
        }
        else
        {
            EasyLogger.Warn("Session not active");
        }

        OnReleaseEvent?.Invoke(session);
    }

    public event SessionEvent? OnActivateEvent;
    public event SessionEvent? OnReleaseEvent;
}
