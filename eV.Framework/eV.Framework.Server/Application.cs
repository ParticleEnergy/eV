// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Framework.Server.Logger;
using eV.Module.GameProfile;
using eV.Module.Routing.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace eV.Framework.Server;

public class Application
{
    private readonly IHost _host;
    public Application(string[] args)
    {
        _host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddSingleton<IHostedService, LifetimeEventsHostedService>();
                services.AddLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddProvider(new LifetimeEventLoggerProvider());
                });
            })
            .Build();
    }

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

    public void Run()
    {
        _host.Run();
    }

    public async Task RunAsync()
    {
        await _host.RunAsync();
    }

    public void Start()
    {
        _host.Start();
    }

    public async Task StartAsync()
    {
        await _host.StartAsync();
    }

    public async Task StopAsync()
    {
        await _host.StopAsync();
    }
}
