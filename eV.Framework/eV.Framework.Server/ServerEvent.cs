// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Routing.Interface;

namespace eV.Framework.Server;

internal static class ServerEvent
{
    public static SessionEvent? ServerOnConnected;
    public static SessionEvent? ServerSessionOnActivate;
    public static SessionEvent? ServerSessionOnRelease;
    public static Action? ServerOnStart;
    public static Action? ServerOnStop;

    public static void OnConnected(ISession session)
    {
        ServerOnConnected?.Invoke(session);
    }

    public static Task SessionOnActivate(ISession session)
    {
        ServerSessionOnActivate?.Invoke(session);
        return Task.CompletedTask;
    }

    public static Task SessionOnRelease(ISession session)
    {
        ServerSessionOnRelease?.Invoke(session);
        return Task.CompletedTask;
    }

    public static void OnStart()
    {
        ServerOnStart?.Invoke();
    }

    public static void OnStop()
    {
        ServerOnStop?.Invoke();
    }
}
