// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Framework.Server.Base;
using eV.Framework.Server.Object;
using eV.Module.Routing.Interface;
using EasyLogger = eV.Module.EasyLog.Logger;

namespace eV.Framework.Server.SystemHandler;

public class ClientSendBroadcastHandler : HandlerBase<ClientSendBroadcastPackage>
{
    protected override async Task Handle(ISession session, ClientSendBroadcastPackage content)
    {
        if (content.Data is not { Length: > 0 })
        {
            EasyLogger.Warn($"Session [{session.SessionId}] send broadcast failed data is empty");
            return;
        }

        if (session.SessionId != null)
            await ServerSession.SendBroadcast(session.SessionId, content.Data);
    }
}
