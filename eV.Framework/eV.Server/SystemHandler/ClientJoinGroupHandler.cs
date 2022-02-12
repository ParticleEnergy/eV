// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.EasyLog;
using eV.Routing.Interface;
using eV.Server.Base;
namespace eV.Server.SystemHandler
{
    public class ClientJoinGroup
    {
        public string? GroupId { get; set; }
        public string? SessionId { get; set; }
    }
    public class ClientJoinGroupHandler : HandlerBase<ClientJoinGroup>
    {
        protected override void Handle(ISession session, ClientJoinGroup content)
        {
            if (content.GroupId is null or "")
            {
                Logger.Warn($"Session {session.SessionId} JoinGroup failed groupId is empty");
                return;
            }
            if (content.SessionId is null or "")
            {
                Logger.Warn($"Session {session.SessionId} JoinGroup failed sessionId is empty");
                return;
            }
            ServerSession.JoinGroup(content.GroupId, content.SessionId);
        }
    }
}
