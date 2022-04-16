// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

namespace eV.Client;

public static class ClientSession
{
    private static Session.Session? s_session;
    public static string? SessionId { get; private set; }
    public static void Init(Session.Session session)
    {
        s_session = session;
    }
    public static bool Send(byte[] data)
    {
        return s_session != null && s_session.Send(data);
    }
    public static bool Send<T>(T data)
    {
        return s_session != null && s_session.Send(data);
    }
    public static bool Send<T>(string sessionId, T data)
    {
        return s_session != null && s_session.Send(sessionId, data);
    }
    public static void SendGroup<T>(string groupId, T data)
    {
        s_session?.SendGroup(groupId, data);
    }
    public static void SendBroadcast<T>(T data)
    {
        s_session?.SendBroadcast(data);
    }
    public static bool JoinGroup(string groupId)
    {
        return s_session != null && s_session.JoinGroup(groupId);
    }
    public static bool LeaveGroup(string groupId)
    {
        return s_session != null && s_session.LeaveGroup(groupId);
    }
    public static void Activate(string sessionId)
    {
        SessionId = sessionId;
        s_session?.Activate(sessionId);
    }
    public static void Activate()
    {
        if (SessionId is null or "")
            return;
        s_session?.Activate(SessionId);
    }
}
