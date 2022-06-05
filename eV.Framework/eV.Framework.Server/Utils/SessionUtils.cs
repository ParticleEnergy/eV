// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using eV.Module.EasyLog;
using eV.Module.Routing.Interface;
using eV.Module.Session;
namespace eV.Framework.Server.Utils;

public static class SessionUtils
{
    public static bool Send(string sessionId, byte[] data)
    {
        Session? session = SessionDispatch.Instance.SessionManager.GetActiveSession(sessionId);
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
            Session? session = SessionDispatch.Instance.SessionManager.GetActiveSession(group.Value);
            if (session?.SessionId == null || session.SessionId.Equals(selfSessionId))
                continue;
            session.Send(data);
        }
    }
    public static void SendBroadcast(string selfSessionId, byte[] data)
    {
        if (SessionDispatch.Instance.SessionManager.GetActiveCount() <= 0)
            return;

        foreach ((string _, Session? session) in SessionDispatch.Instance.SessionManager.GetAllActiveSession())
        {
            if (session.SessionId == null || session.SessionId.Equals(selfSessionId))
                continue;
            session.Send(data);
        }
    }
    public static bool Activate(ISession session)
    {
        return SessionDispatch.Instance.SessionManager.AddActiveSession((Session)session);
    }
    public static bool Release(ISession session)
    {
        return SessionDispatch.Instance.SessionManager.RemoveActiveSession((Session)session);
    }

    public static bool SendAction(string sessionId, byte[] data)
    {
        Session? session = SessionDispatch.Instance.SessionManager.GetActiveSession(sessionId);
        return session != null && session.Send(data);
    }

    public static void SendGroupAction(string groupId, byte[] data)
    {
        ConcurrentDictionary<string, string>? groups = SessionDispatch.Instance.SessionGroup.GetGroup(groupId);
        if (groups == null)
        {
            Logger.Warn($"Group {groupId} not found");
            return;
        }
        foreach (KeyValuePair<string, string> group in groups)
        {
            Session? session = SessionDispatch.Instance.SessionManager.GetActiveSession(group.Value);
            if (session?.SessionId == null)
                continue;
            session.Send(data);
        }
    }

    public static void SendBroadcastAction(byte[] data)
    {
        if (SessionDispatch.Instance.SessionManager.GetActiveCount() <= 0)
            return;

        foreach ((string _, Session? session) in SessionDispatch.Instance.SessionManager.GetAllActiveSession())
        {
            if (session.SessionId == null)
                continue;
            session.Send(data);
        }
    }
}
