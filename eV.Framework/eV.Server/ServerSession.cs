// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using eV.EasyLog;
using eV.Routing.Interface;
namespace eV.Server;

public static class ServerSession
{
    public static bool Send(string sessionId, byte[] data)
    {
        Session.Session? session = SessionDispatch.Instance.SessionManager.GetActiveSession(sessionId);
        return session != null && session.Send(data);

    }
    public static void SendGroup(string selfSessionId, string groupId, byte[] data)
    {
        ConcurrentDictionary<string, string>? groups = SessionDispatch.Instance.SessionGroup.GetGroup(groupId);
        if (groups == null)
        {
            Logger.Warn($"Group {groupId} not found");
            return;
        }
        foreach (KeyValuePair<string, string> group in groups)
        {
            Session.Session? session = SessionDispatch.Instance.SessionManager.GetActiveSession(group.Value);
            if (session?.SessionId == null || session.SessionId.Equals(selfSessionId))
                continue;
            session.Send(data);
        }
    }
    public static void SendBroadcast(string selfSessionId, byte[] data)
    {
        if (SessionDispatch.Instance.SessionManager.GetActiveCount() <= 0)
            return;

        foreach ((string _, Session.Session? session) in SessionDispatch.Instance.SessionManager.GetAllActiveSession())
        {
            if (session.SessionId == null || session.SessionId.Equals(selfSessionId))
                continue;
            session.Send(data);
        }
    }
    public static bool JoinGroup(string groupId, string sessionId)
    {
        return SessionDispatch.Instance.SessionGroup.JoinGroup(groupId, sessionId);
    }
    public static bool LeaveGroup(string groupId, string sessionId)
    {
        return SessionDispatch.Instance.SessionGroup.LeaveGroup(groupId, sessionId);
    }
    public static bool Activate(ISession session)
    {
        return SessionDispatch.Instance.SessionManager.AddActiveSession((Session.Session)session);
    }
    public static bool Release(ISession session)
    {
        return SessionDispatch.Instance.SessionManager.RemoveActiveSession((Session.Session)session);
    }
}
