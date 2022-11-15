// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Framework.Server.Base;
using eV.Module.Routing.Interface;
using EasyLogger = eV.Module.EasyLog.Logger;

namespace eV.Framework.Server.SystemHandler;

public class ClientJoinGroup
{
    public string GroupId { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
}

public class ClientJoinGroupHandler : HandlerBase<ClientJoinGroup>
{
    protected override void Handle(ISession session, ClientJoinGroup content)
    {
        if (content.GroupId.Equals(""))
        {
            EasyLogger.Warn($"Session {session.SessionId} JoinGroup failed groupId is empty");
            return;
        }

        if (content.SessionId.Equals(""))
        {
            EasyLogger.Warn($"Session {session.SessionId} JoinGroup failed sessionId is empty");
            return;
        }

        ServerSession.JoinGroup(content.GroupId, content.SessionId);
    }
}
