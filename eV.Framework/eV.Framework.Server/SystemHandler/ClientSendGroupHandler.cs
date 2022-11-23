// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Framework.Server.Base;
using eV.Module.Routing.Interface;
using EasyLogger = eV.Module.EasyLog.Logger;

namespace eV.Framework.Server.SystemHandler;

public class ClientSendGroup
{
    public string GroupId { get; set; } = string.Empty;
    public byte[]? Data { get; set; } = null;
}

public class ClientSendGroupHandler : HandlerBase<ClientSendGroup>
{
    protected override Task Handle(ISession session, ClientSendGroup content)
    {
        if (content.GroupId.Equals(""))
        {
            EasyLogger.Warn($"Session [{session.SessionId}] SendGroup failed groupId is empty");
            return Task.CompletedTask;
        }

        if (content.Data is not { Length: > 0 })
        {
            EasyLogger.Warn($"Session [{session.SessionId}] SendGroup failed data is empty");
            return Task.CompletedTask;
        }

        ServerSession.SendGroup(session.SessionId!, content.GroupId, content.Data);
        return Task.CompletedTask;
    }
}
