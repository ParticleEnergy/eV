// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.EasyLog;
using eV.Routing.Interface;
using eV.Server.Base;
namespace eV.Server.SystemHandler;

public class ClientKeepalive
{
}
public class ClientKeepaliveHandler : HandlerBase<ClientKeepalive>
{
    public ClientKeepaliveHandler()
    {
        Skip = true;
    }
    protected override void Handle(ISession session, ClientKeepalive _)
    {
        Logger.Info($"Session {session.SessionId} Connected Time - {session.ConnectedDateTime:hh:mm:ss} keepalive time - {DateTime.Now:hh:mm:ss}");
    }
}
