// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using eV.Routing.Interface;
using eV.Server.Base;
namespace eV.Server.ClientHeartbeat
{
    public class KeepaliveHandler : HandlerBase<Keepalive>
    {
        protected override void Handle(ISession session, Keepalive _)
        {
            Logger.Info($"Session {session.SessionId} Connected Time - {session.ConnectedDateTime:hh:mm:ss} keepalive time - {DateTime.Now:hh:mm:ss}");
        }
    }
}
