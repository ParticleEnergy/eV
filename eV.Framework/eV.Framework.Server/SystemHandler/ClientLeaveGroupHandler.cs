// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Framework.Server.Base;
using eV.Module.Routing.Interface;
using EasyLogger = eV.Module.EasyLog.Logger;

namespace eV.Framework.Server.SystemHandler;

public class ClientLeaveGroup
{
    public string GroupId { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
}

public class ClientLeaveGroupHandler : HandlerBase<ClientLeaveGroup>
{
    protected override Task Handle(ISession session, ClientLeaveGroup content)
    {
        if (content.GroupId.Equals(""))
        {
            EasyLogger.Warn($"Session {session.SessionId} LeaveGroup failed groupId is empty");
            return Task.CompletedTask;
        }

        if (content.SessionId.Equals(""))
        {
            EasyLogger.Warn($"Session {session.SessionId} LeaveGroup failed sessionId is empty");
            return Task.CompletedTask;
        }

        ServerSession.LeaveGroup(content.GroupId, content.SessionId);
        return Task.CompletedTask;
    }
}
