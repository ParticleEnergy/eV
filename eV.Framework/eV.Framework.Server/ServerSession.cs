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

    public static void SendBroadcast(string selfSessionId, byte[] data)
    {
        s_sessionDrive?.SendBroadcast(selfSessionId, data);
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
