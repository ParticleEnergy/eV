// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Framework.Server.Logger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace eV.Framework.Server;

public class Builder
{
    private readonly IHostBuilder _builder;
    private Action<IServiceCollection>? _configureServicesDelegate;
    public Builder(string[]? args)
    {
        _builder = args == null ? Host.CreateDefaultBuilder() : Host.CreateDefaultBuilder(args);
    }

    public Builder ConfigureServices(Action<IServiceCollection> configureDelegate)
    {
        _configureServicesDelegate = configureDelegate;
        return this;
    }

    public Application Build()
    {
        _builder.ConfigureServices(services =>
        {
            services.AddSingleton<IHostedService, HostedService>();
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(new ServerLoggerProvider());
            });
            _configureServicesDelegate?.Invoke(services);
        });

        return new Application(_builder.Build());
    }
}
