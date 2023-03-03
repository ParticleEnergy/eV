// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Routing.Interface;
using eV.Module.Session;

namespace eV.Framework.Server;

public class SessionDispatch
{
    public static SessionDispatch Instance { get; } = new();
    public SessionManager SessionManager { get; }

    private SessionDispatch()
    {
        SessionManager = new SessionManager();
    }

    public bool Send(string sessionId, byte[] data)
    {
        Session? session = SessionManager.GetActiveSession(sessionId);
        return session != null && session.Send(data);
    }

    public void SendBroadcast(string selfSessionId, byte[] data)
    {
        if (SessionManager.GetActiveCount() <= 0)
            return;

        foreach ((string _, Session? session) in SessionManager.GetAllActiveSession())
        {
            if (session.SessionId == null || session.SessionId.Equals(selfSessionId))
                continue;
            session.Send(data);
        }
    }

    public bool Activate(ISession session)
    {
        return SessionManager.AddActiveSession((Session)session);
    }

    public bool Release(ISession session)
    {
        return SessionManager.RemoveActiveSession((Session)session);
    }
}
