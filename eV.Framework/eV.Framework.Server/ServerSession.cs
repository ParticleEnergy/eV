// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Framework.Server.Object;
using eV.Module.Cluster;
using eV.Module.Routing.Interface;
using eV.Module.Session;

namespace eV.Framework.Server;

public static class ServerSession
{
    public static async Task<bool> Send(string sessionId, byte[] data)
    {
        if (SessionDispatch.Instance.Send(sessionId, data))
        {
            return true;
        }

        if (CommunicationManager.Instance == null) return false;

        string nodeId = await CommunicationManager.Instance.SessionRegistrationAuthority.GetNodeId(sessionId);
        if (nodeId.Equals(""))
            return false;

        return await CommunicationManager.Instance.SendInternalMessage(nodeId, new SendBySessionIdPackage { SessionId = sessionId, Data = data });
    }

    public static async Task SendBroadcast(string selfSessionId, byte[] data)
    {
        if (CommunicationManager.Instance == null)
        {
            SessionDispatch.Instance.SendBroadcast(selfSessionId, data);
        }
        else
        {
            await CommunicationManager.Instance.SendInternalMessage(new InternalSendBroadcastPackage { SelfSessionId = selfSessionId, Data = data });
        }
    }

    public static bool Activate(ISession session)
    {
        if (!SessionDispatch.Instance.Activate((Session)session))
            return false;

        if (CommunicationManager.Instance != null)
        {
            CommunicationManager.Instance.SessionRegistrationAuthority.Registry(session.SessionId!);
        }

        return true;
    }

    public static bool Release(ISession session)
    {
        if (!SessionDispatch.Instance.Release((Session)session))
            return false;

        if (CommunicationManager.Instance != null)
        {
            CommunicationManager.Instance.SessionRegistrationAuthority.Deregister(session.SessionId!);
        }

        return true;
    }
}
