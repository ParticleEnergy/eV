// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Framework.Server;
using eV.Module.GameProfile;
using Microsoft.Extensions.Hosting;
namespace eV.ServerExample.Core;

internal class LifetimeEventsHostedService : IHostedService
{

    private readonly IHostApplicationLifetime _appLifetime;
    private readonly Server _server;

    public LifetimeEventsHostedService(IHostApplicationLifetime appLifetime)
    {
        _appLifetime = appLifetime;

        Profile.OnLoad += delegate
        {
            Profile.AssignmentConfigObject(GameProfile.Instance);
        };

        _server = new Server();
        _server.OnConnected += CustomServerEvent.OnConnected;
        _server.SessionOnActivate += CustomServerEvent.SessionOnActivate;
        _server.SessionOnRelease += CustomServerEvent.SessionOnRelease;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _appLifetime.ApplicationStarted.Register(OnStarted);
        _appLifetime.ApplicationStopped.Register(OnStopped);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void OnStarted()
    {
        _server.Start();
    }

    private void OnStopped()
    {
        _server.Stop();
    }
}
