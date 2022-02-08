// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.


using System.Collections.Concurrent;
using eV.Network.Core;
using eV.Session.Interface;
using log4net;
namespace eV.Session
{
    public class SessionManager
    {
        private readonly ConcurrentDictionary<string, Session> _sessions = new();
        private readonly ConcurrentDictionary<string, Session> _activeSessions = new();
        private readonly ILog _logger = LogManager.GetLogger(DefaultSetting.LoggerName);

        #region Session
        public Session GetSession(Channel channel, SessionExtend? sessionExtend)
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
                _logger.Info($"Channel {channel.ChannelId} bind session success");
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
                _logger.Warn("SessionId is null");
                return false;
            }
            if (session.SessionState != SessionState.Active)
            {
                _logger.Warn("Session is not active");
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
}
