// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.Routing.Interface;
namespace eV.Session.Interface
{
    public interface SessionExtend
    {
        public bool Send(string sessionId, byte[] data);
        public void SendGroup(string groupName, byte[] data);
        public void SendBroadcast(byte[] data);
        public bool JoinGroup(string groupName, string sessionId);
        public bool LeaveGroup(string groupName, string sessionId);
        public void OnActivate(ISession session);
        public void OnRelease(ISession session);
    }
}
