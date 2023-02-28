// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Routing.Interface;
using eV.Module.Session.Interface;
using EasyLogger = eV.Module.EasyLog.Logger;

namespace eV.Framework.Server;

public class SessionExtension : ISessionExtend
{
    public event SessionEvent? OnActivateEvent;
    public event SessionEvent? OnReleaseEvent;

    public async Task<bool> Send(string sessionId, byte[] data)
    {
        return await ServerSession.Instance.Send(sessionId, data);
    }

    public async void SendBroadcast(string selfSessionId, byte[] data)
    {
        await ServerSession.Instance.SendBroadcast(selfSessionId, data);
    }

    public void OnActivate(ISession session)
    {
        if (ServerSession.Instance.Activate(session))
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
            if (!ServerSession.Instance.Release(session))
                EasyLogger.Warn($"Session {session.SessionId} Session remove active group error");
        }
        else
        {
            EasyLogger.Warn("Session not active");
        }

        OnReleaseEvent?.Invoke(session);
    }
}
