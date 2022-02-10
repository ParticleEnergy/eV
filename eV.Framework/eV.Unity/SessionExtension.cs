// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.Routing.Interface;
using eV.Session.Interface;
namespace eV.Unity
{
    public class SessionExtension : ISessionExtend
    {
        public event SessionEvent? OnActivateEvent;
        public event SessionEvent? OnReleaseEvent;

        public SessionExtension()
        {

        }
        public bool Send(string sessionId, byte[] data)
        {
            return false;
        }
        public void SendGroup(string selfSessionId, string groupId, byte[] data)
        {
        }
        public void SendBroadcast(string selfSessionId, byte[] data)
        {
        }
        public bool JoinGroup(string groupId, string sessionId)
        {
            return false;
        }
        public bool LeaveGroup(string groupId, string sessionId)
        {
            return false;
        }
        public void OnActivate(ISession session)
        {
        }
        public void OnRelease(ISession session)
        {
        }
    }
}
