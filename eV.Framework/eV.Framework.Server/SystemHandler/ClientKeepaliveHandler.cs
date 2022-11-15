// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Framework.Server.Base;
using eV.Module.Routing.Interface;
using EasyLogger = eV.Module.EasyLog.Logger;

namespace eV.Framework.Server.SystemHandler;

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
        EasyLogger.Info(
            $"Session {session.SessionId} Connected Time - {session.ConnectedDateTime:hh:mm:ss} keepalive time - {DateTime.Now:hh:mm:ss}");
    }
}
