// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Framework.Server.Interface;
using eV.Framework.Server.Utils;
using eV.Module.Routing.Interface;

namespace eV.Framework.Server.SessionDrive;

public class SingleSessionDrive : ISessionDrive
{
    public bool Send(string sessionId, byte[] data)
    {
        return SessionUtils.Send(sessionId, data);
    }

    public void SendGroup(string selfSessionId, string groupId, byte[] data)
    {
        SessionUtils.SendGroup(selfSessionId, groupId, data);
    }

    public void SendBroadcast(string selfSessionId, byte[] data)
    {
        SessionUtils.SendBroadcast(selfSessionId, data);
    }

    public bool Activate(ISession session)
    {
        return SessionUtils.Activate(session);
    }

    public bool Release(ISession session)
    {
        return SessionUtils.Release(session);
    }

    #region Group

    public bool CreateGroup(string groupId)
    {
        return SessionUtils.CreateGroup(groupId);
    }

    public bool DeleteGroup(string groupId)
    {
        return SessionUtils.DeleteGroup(groupId);
    }

    #endregion
}
