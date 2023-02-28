// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Cluster;
using eV.Module.Routing.Interface;
using eV.Module.Session;

namespace eV.Framework.Server;

public class ServerSession
{
    public static ServerSession Instance { get; } = new();

    private ServerSession()
    {
    }

    public async Task<bool> Send(string sessionId, byte[] data)
    {
        Session? session = SessionDispatch.Instance.SessionManager.GetActiveSession(sessionId);
        if (session != null)
            return session.Send(data);

        if (CommunicationManager.Instance != null)
        {
            return await CommunicationManager.Instance.Send(sessionId, data);
        }

        return false;
    }

    public async Task SendBroadcast(string selfSessionId, byte[] data)
    {
        if (SessionDispatch.Instance.SessionManager.GetActiveCount() <= 0)
            return;

        foreach ((string _, Session? session) in SessionDispatch.Instance.SessionManager.GetAllActiveSession())
        {
            if (session.SessionId == null || session.SessionId.Equals(selfSessionId))
                continue;
            session.Send(data);
        }

        if (CommunicationManager.Instance != null)
        {
            await CommunicationManager.Instance.SendBroadcast(data);
        }
    }

    public bool Activate(ISession session)
    {
        if (!SessionDispatch.Instance.SessionManager.AddActiveSession((Session)session))
            return false;

        if (CommunicationManager.Instance != null)
        {
            CommunicationManager.Instance.SessionRegistrationAuthority.Registry(session.SessionId!);
        }

        return true;
    }

    public bool Release(ISession session)
    {
        if (!SessionDispatch.Instance.SessionManager.RemoveActiveSession((Session)session))
            return false;

        if (CommunicationManager.Instance != null)
        {
            CommunicationManager.Instance.SessionRegistrationAuthority.Deregister(session.SessionId!);
        }

        return true;
    }
}
