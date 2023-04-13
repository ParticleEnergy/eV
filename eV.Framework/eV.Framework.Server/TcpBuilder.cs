// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Framework.Server.Logger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace eV.Framework.Server;

public class TcpBuilder
{
    private readonly IHostBuilder _builder;
    private Action<IServiceCollection>? _configureServicesDelegate;

    public TcpBuilder(string[] args)
    {
        _builder = Host.CreateDefaultBuilder(args);
    }

    public TcpBuilder ConfigureServices(Action<IServiceCollection> configureDelegate)
    {
        _configureServicesDelegate = configureDelegate;
        return this;
    }

    public TcpApplication Build()
    {
        _builder.ConfigureServices(services =>
        {
            services.AddSingleton<IHostedService, TcpHostedService>();
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(new ServerLoggerProvider());
            });
            _configureServicesDelegate?.Invoke(services);
        });

        return new TcpApplication(_builder.Build());
    }
}
