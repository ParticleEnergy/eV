// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.EasyLog;
using eV.Routing.Interface;
using eV.Server.Base;
namespace eV.Server.SystemHandler
{
    public class ClientSendBySessionId
    {
        public string? SessionId { get; set; }
        public byte[]? Data { get; set; }
    }

    public class ClientSendBySessionIdHandler : HandlerBase<ClientSendBySessionId>
    {
        protected override void Handle(ISession session, ClientSendBySessionId content)
        {
            if (content.SessionId is null or "")
            {
                Logger.Warn($"Session {session.SessionId} send by sessionId failed sessionId is empty");
                return;
            }
            if (content.Data is not { Length: > 0 })
            {
                Logger.Warn($"Session {session.SessionId} send by sessionId failed data is empty");
                return;
            }
            ServerSession.Send(content.SessionId, content.Data);
        }
    }
}
