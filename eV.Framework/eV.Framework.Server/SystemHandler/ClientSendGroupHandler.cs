// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.Framework.Server.Base;
using eV.Module.EasyLog;
using eV.Module.Routing.Interface;
namespace eV.Framework.Server.SystemHandler;

public class ClientSendGroup
{
    public string? GroupId { get; set; }
    public byte[]? Data { get; set; }
}
public class ClientSendGroupHandler : HandlerBase<ClientSendGroup>
{
    protected override void Handle(ISession session, ClientSendGroup content)
    {
        if (content.GroupId is null or "")
        {
            Logger.Warn($"Session {session.SessionId} SendGroup failed groupId is empty");
            return;
        }
        if (content.Data is not { Length: > 0 })
        {
            Logger.Warn($"Session {session.SessionId} SendGroup failed data is empty");
            return;
        }
        ServerSession.SendGroup(session.SessionId!, content.GroupId, content.Data);
    }
}
