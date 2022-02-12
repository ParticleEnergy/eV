// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.EasyLog;
using eV.Routing.Interface;
using eV.Server.Base;
namespace eV.Server.SystemHandler;

public class ClientSendBroadcast
{
    public byte[]? Data { get; set; }
}
public class ClientSendBroadcastHandler : HandlerBase<ClientSendBroadcast>
{
    protected override void Handle(ISession session, ClientSendBroadcast content)
    {
        if (content.Data is not { Length: > 0 })
        {
            Logger.Warn($"Session {session.SessionId} send broadcast failed data is empty");
            return;
        }
        ServerSession.SendBroadcast(session.SessionId!, content.Data);
    }
}
