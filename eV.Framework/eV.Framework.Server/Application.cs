// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.GameProfile;
using eV.Module.Routing.Interface;
using Microsoft.Extensions.Hosting;
namespace eV.Framework.Server;

public sealed class Application
{
    private readonly IHost _host;
    public Application(IHost host)
    {
        _host = host;
    }

    public void Run()
    {
        _host.Run();
    }

    public async Task RunAsync()
    {
        await _host.RunAsync();
    }

    public static TcpAndHttpBuilder CreateBuilder(string[]? args)
    {
        return new TcpAndHttpBuilder(args);
    }

    public static TcpBuilder CreateOnlyTcpBuilder(string[] args)
    {
        return new TcpBuilder(args);
    }

    #region
    public Application SetProfile<T>(T data)
    {
        Profile.OnLoad += delegate
        {
            Profile.AssignmentConfigObject(data);
        };
        return this;
    }

    public Application SetServerOnConnected(SessionEvent onConnected)
    {
        ServerEvent.ServerOnConnected = onConnected;
        return this;
    }

    public Application SetServerSessionOnActivate(SessionEvent sessionOnActivate)
    {
        ServerEvent.ServerSessionOnActivate = sessionOnActivate;
        return this;
    }

    public Application SetServerSessionOnRelease(SessionEvent sessionOnRelease)
    {
        ServerEvent.ServerSessionOnRelease = sessionOnRelease;
        return this;
    }
    #endregion
}
