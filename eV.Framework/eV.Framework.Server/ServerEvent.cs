// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Routing.Interface;
namespace eV.Framework.Server;

internal static class ServerEvent
{
    public static SessionEvent? ServerOnConnected;
    public static SessionEvent? ServerSessionOnActivate;
    public static SessionEvent? ServerSessionOnRelease;

    public static void OnConnected(ISession session)
    {
        ServerOnConnected?.Invoke(session);
    }

    public static void SessionOnActivate(ISession session)
    {
        ServerSessionOnActivate?.Invoke(session);
    }

    public static void SessionOnRelease(ISession session)
    {
        ServerSessionOnRelease?.Invoke(session);
    }
}
