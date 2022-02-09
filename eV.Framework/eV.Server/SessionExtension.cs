// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.Routing.Interface;
using eV.Session;
using eV.Session.Interface;
using log4net;
namespace eV.Server
{
    public class SessionExtension : ISessionExtend
    {
        public event SessionEvent? OnActivateEvent;
        public event SessionEvent? OnReleaseEvent;

        private readonly SessionManager _sessionManager;
        private readonly SessionGroup _sessionGroup;

        private readonly ILog _logger = LogManager.GetLogger(DefaultSetting.LoggerName);

        public SessionExtension(SessionManager sessionManager, SessionGroup sessionGroup)
        {
            _sessionManager = sessionManager;
            _sessionGroup = sessionGroup;
        }
        public bool Send(string sessionId, byte[] data)
        {
            var session = _sessionManager.GetActiveSession(sessionId);
            return session != null && session.Send(data);
        }
        public void SendGroup(string groupName, byte[] data)
        {
            var groups = _sessionGroup.GetGroup(groupName);
            if (groups == null)
                return;
            foreach (var group in groups)
            {
                _sessionManager.GetActiveSession(group.Value)?.Send(data);
            }
        }
        public void SendBroadcast(byte[] data)
        {
            if (_sessionManager.GetActiveCount() <= 0)
                return;

            foreach (var (_, session) in _sessionManager.GetAllActiveSession())
            {
                session.Send(data);
            }
        }
        public bool JoinGroup(string groupName, string sessionId)
        {
            return _sessionGroup.JoinGroup(groupName, sessionId);
        }
        public bool LeaveGroup(string groupName, string sessionId)
        {
            return _sessionGroup.LeaveGroup(groupName, sessionId);
        }
        public void OnActivate(ISession session)
        {
            if (_sessionManager.AddActiveSession((Session.Session)session))
            {
                OnActivateEvent?.Invoke(session);
            }
            else
            {
                _logger.Error($"Session {session.SessionId} Session add active group error");
                session.Shutdown();
            }
        }
        public void OnRelease(ISession session)
        {
            if (!_sessionManager.RemoveActiveSession((Session.Session)session))
                _logger.Error($"Session {session.SessionId} Session remove active group error");
            OnReleaseEvent?.Invoke(session);
        }
    }
}
