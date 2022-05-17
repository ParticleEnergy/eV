// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.


using System.Collections.Concurrent;
using eV.EasyLog;
using eV.Network.Core.Interface;
using eV.Session.Interface;
namespace eV.Session;

public class SessionManager
{
    private readonly ConcurrentDictionary<string, Session> _activeSessions = new();
    private readonly ConcurrentDictionary<string, Session> _sessions = new();

    #region Session
    public Session GetSession(ITcpChannel channel, ISessionExtend? sessionExtend)
    {
        Session session;
        if (_sessions.TryGetValue(channel.ChannelId, out Session? result))
        {
            session = result;
        }
        else
        {
            session = new Session(channel);
            if (sessionExtend != null)
            {
                session.SendAction = sessionExtend.Send;
                session.SendGroupAction = sessionExtend.SendGroup;
                session.SendBroadcastAction = sessionExtend.SendBroadcast;
                session.JoinGroupAction = sessionExtend.JoinGroup;
                session.LeaveGroupAction = sessionExtend.LeaveGroup;
                session.OnActivate += sessionExtend.OnActivate;
                session.OnRelease += sessionExtend.OnRelease;
            }
            _sessions[channel.ChannelId] = session;
            Logger.Info($"Channel {channel.ChannelId} bind session success");
        }
        session.Occupy();
        return session;
    }
    public Session? GetSession(string channelId)
    {
        return _sessions.TryGetValue(channelId, out Session? result) ? result : null;
    }
    public ConcurrentDictionary<string, Session> GetAllSessions()
    {
        return _sessions;
    }
    public int GetAllSessionsCount()
    {
        return _sessions.Count;
    }
    #endregion

    #region Active
    public Session? GetActiveSession(string sessionId)
    {
        return _activeSessions.TryGetValue(sessionId, out Session? result) && result.SessionState == SessionState.Active ? result : null;
    }
    public ConcurrentDictionary<string, Session> GetAllActiveSession()
    {
        return _activeSessions;
    }
    public int GetActiveCount()
    {
        return _activeSessions.Count;
    }
    public bool AddActiveSession(Session session)
    {
        if (session.SessionId is null or "")
        {
            Logger.Warn("SessionId is null");
            return false;
        }
        if (session.SessionState != SessionState.Active)
        {
            Logger.Warn("Session is not active");
            return false;
        }

        _activeSessions[session.SessionId] = session;
        return true;
    }
    public bool RemoveActiveSession(Session session)
    {
        return session.SessionId is not (null or "") && _activeSessions.TryRemove(session.SessionId, out Session? _);
    }
    #endregion
}
