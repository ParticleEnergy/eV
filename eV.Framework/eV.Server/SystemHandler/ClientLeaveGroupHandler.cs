// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.EasyLog;
using eV.Routing.Interface;
using eV.Server.Base;
namespace eV.Server.SystemHandler
{
    public class ClientLeaveGroup
    {
        public string? GroupId { get; set; }
        public string? SessionId { get; set; }
    }
    public class ClientLeaveGroupHandler : HandlerBase<ClientLeaveGroup>
    {
        protected override void Handle(ISession session, ClientLeaveGroup content)
        {
            if (content.GroupId is null or "")
            {
                Logger.Warn($"Session {session.SessionId} LeaveGroup failed groupId is empty");
                return;
            }
            if (content.SessionId is null or "")
            {
                Logger.Warn($"Session {session.SessionId} LeaveGroup failed sessionId is empty");
                return;
            }
            ServerSession.LeaveGroup(content.GroupId, content.SessionId);
        }
    }
}
