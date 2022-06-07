// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using Microsoft.Extensions.Hosting;
namespace eV.Framework.Server;

public class LifetimeEventsHostedService : IHostedService
{

    private readonly IHostApplicationLifetime _appLifetime;
    private readonly Server _server;

    public LifetimeEventsHostedService(IHostApplicationLifetime appLifetime)
    {
        _appLifetime = appLifetime;
        _server = new Server();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _appLifetime.ApplicationStarted.Register(delegate
        {
            _server.Start();
        });
        _appLifetime.ApplicationStopped.Register(delegate
        {
            _server.Stop();
        });
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
