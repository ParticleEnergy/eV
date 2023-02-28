// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Framework.Server.Base;
using eV.Module.Routing.Interface;
using EasyLogger = eV.Module.EasyLog.Logger;

namespace eV.Framework.Server.SystemHandler;

public class ClientSendBroadcast
{
    public byte[]? Data { get; set; } = null;
}

public class ClientSendBroadcastHandler : HandlerBase<ClientSendBroadcast>
{
    protected override async Task Handle(ISession session, ClientSendBroadcast content)
    {
        if (content.Data is not { Length: > 0 })
        {
            EasyLogger.Warn($"Session [{session.SessionId}] send broadcast failed data is empty");
            return;
        }

        await ServerSession.Instance.SendBroadcast(session.SessionId!, content.Data);
    }
}
