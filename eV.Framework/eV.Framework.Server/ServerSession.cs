// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Framework.Server.Interface;
using eV.Module.EasyLog;
using eV.Module.Routing.Interface;
namespace eV.Framework.Server;

public static class ServerSession
{
    private static ISessionDrive? _sessionDrive;

    public static void SetSessionDrive(ISessionDrive sessionDrive)
    {
        if (_sessionDrive != null)
        {
            Logger.Error("Can only be set at startup");
            return;
        }
        _sessionDrive = sessionDrive;
    }

    public static bool Send(string sessionId, byte[] data)
    {
        return _sessionDrive?.Send(sessionId, data) ?? false;
    }
    public static void SendGroup(string selfSessionId, string groupId, byte[] data)
    {
        _sessionDrive?.SendGroup(selfSessionId, groupId, data);
    }
    public static void SendBroadcast(string selfSessionId, byte[] data)
    {
        _sessionDrive?.SendBroadcast(selfSessionId, data);
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
        return _sessionDrive?.Activate(session) ?? false;
    }
    public static bool Release(ISession session)
    {
        return _sessionDrive?.Release(session) ?? false;
    }
}
