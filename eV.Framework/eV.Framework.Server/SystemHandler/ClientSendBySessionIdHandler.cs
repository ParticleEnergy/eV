// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Framework.Server.Base;
using eV.Module.Routing.Interface;
using EasyLogger = eV.Module.EasyLog.Logger;

namespace eV.Framework.Server.SystemHandler;

public class ClientSendBySessionId
{
    public string SessionId { get; set; } = string.Empty;
    public byte[]? Data { get; set; } = null;
}

public class ClientSendBySessionIdHandler : HandlerBase<ClientSendBySessionId>
{
    protected override Task Handle(ISession session, ClientSendBySessionId content)
    {
        if (content.SessionId.Equals(""))
        {
            EasyLogger.Warn($"Session {session.SessionId} send by sessionId failed sessionId is empty");
            return Task.CompletedTask;
        }

        if (content.Data is not { Length: > 0 })
        {
            EasyLogger.Warn($"Session {session.SessionId} send by sessionId failed data is empty");
            return Task.CompletedTask;
        }

        ServerSession.Send(content.SessionId, content.Data);
        return Task.CompletedTask;
    }
}
