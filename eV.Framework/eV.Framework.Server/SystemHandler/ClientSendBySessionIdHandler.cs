// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Framework.Server.Base;
using eV.Framework.Server.Object;
using eV.Module.Routing.Interface;
using EasyLogger = eV.Module.EasyLog.Logger;

namespace eV.Framework.Server.SystemHandler;

public class ClientSendBySessionIdHandler : HandlerBase<SendBySessionIdPackage>
{
    protected override async Task Handle(ISession session, SendBySessionIdPackage content)
    {
        if (content.SessionId.Equals(""))
        {
            EasyLogger.Warn($"Session [{session.SessionId}] send by sessionId failed sessionId is empty");
            return;
        }

        if (content.Data is not { Length: > 0 })
        {
            EasyLogger.Warn($"Session [{session.SessionId}] send by sessionId failed data is empty");
            return;
        }

        await ServerSession.Send(content.SessionId, content.Data);
    }
}
