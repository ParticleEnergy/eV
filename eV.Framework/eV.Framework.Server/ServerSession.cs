// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using eV.Framework.Server.Interface;
using eV.Module.Routing.Interface;
using EasyLogger = eV.Module.EasyLog.Logger;

namespace eV.Framework.Server;

public static class ServerSession
{
    private static ISessionDrive? s_sessionDrive;

    public static void SetSessionDrive(ISessionDrive sessionDrive)
    {
        if (s_sessionDrive != null)
        {
            EasyLogger.Error("Can only be set at startup");
            return;
        }

        s_sessionDrive = sessionDrive;
    }

    public static bool Send(string sessionId, byte[] data)
    {
        return s_sessionDrive?.Send(sessionId, data) ?? false;
    }

    public static void SendGroup(string selfSessionId, string groupId, byte[] data)
    {
        s_sessionDrive?.SendGroup(selfSessionId, groupId, data);
    }

    public static void SendBroadcast(string selfSessionId, byte[] data)
    {
        s_sessionDrive?.SendBroadcast(selfSessionId, data);
    }

    public static bool JoinGroup(string groupId, string sessionId)
    {
        return SessionDispatch.Instance.SessionGroup.JoinGroup(groupId, sessionId);
    }

    public static bool LeaveGroup(string groupId, string sessionId)
    {
        return SessionDispatch.Instance.SessionGroup.LeaveGroup(groupId, sessionId);
    }

    public static ConcurrentDictionary<string, string>? GetGroup(string groupId)
    {
        return SessionDispatch.Instance.SessionGroup.GetGroup(groupId);
    }

    public static bool CreateGroup(string groupId)
    {
        return SessionDispatch.Instance.SessionGroup.CreateGroup(groupId);
    }

    public static bool DeleteGroup(string groupId)
    {
        return SessionDispatch.Instance.SessionGroup.DeleteGroup(groupId);
    }

    public static bool Activate(ISession session)
    {
        return s_sessionDrive?.Activate(session) ?? false;
    }

    public static bool Release(ISession session)
    {
        return s_sessionDrive?.Release(session) ?? false;
    }
}
