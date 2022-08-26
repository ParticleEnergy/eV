// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Framework.Server.Logger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using EasyLogger = eV.Module.EasyLog.Logger;
namespace eV.Framework.Server;

public abstract class Builder
{
    protected Builder()
    {
        EasyLogger.SetLogger(new ServerLog(Configure.Instance.ProjectName));
        EasyLogger.Info(DefaultSetting.Logo);
    }
    public abstract Application Build();
}
public class TcpBuilder : Builder
{
    private readonly IHostBuilder _builder;
    private Action<IServiceCollection>? _configureServicesDelegate;
    public TcpBuilder(string[]? args)
    {
        _builder = args == null ? Host.CreateDefaultBuilder() : Host.CreateDefaultBuilder(args);
    }

    public TcpBuilder ConfigureServices(Action<IServiceCollection> configureDelegate)
    {
        _configureServicesDelegate = configureDelegate;
        return this;
    }

    public override Application Build()
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

        return new Application(_builder.Build());
    }
}
public class TcpAndHttpBuilder : Builder
{
    private readonly WebApplicationBuilder _builder;
    private Action<WebApplication>? _configureWebApplicationDelegate;
    public TcpAndHttpBuilder(string[]? args)
    {
        _builder = args == null ? WebApplication.CreateBuilder() : WebApplication.CreateBuilder(args);
        _builder.Services.AddSingleton<IHostedService, TcpHostedService>();
        _builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddProvider(new ServerLoggerProvider());
        });
        _builder.Services.AddControllers();
        _builder.Services.AddEndpointsApiExplorer();
        _builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = Configure.Instance.ProjectName
            });
        });
    }

    public TcpAndHttpBuilder ConfigureConfiguration(Action<ConfigurationManager> configureDelegate)
    {
        configureDelegate(_builder.Configuration);
        return this;
    }

    public TcpAndHttpBuilder ConfigureEnvironment(Action<IWebHostEnvironment> configureDelegate)
    {
        configureDelegate(_builder.Environment);
        return this;
    }

    public TcpAndHttpBuilder ConfigureWebHost(Action<ConfigureWebHostBuilder> configureDelegate)
    {
        configureDelegate(_builder.WebHost);
        return this;
    }

    public TcpAndHttpBuilder ConfigureServices(Action<IServiceCollection> configureDelegate)
    {
        configureDelegate(_builder.Services);
        return this;
    }

    public TcpAndHttpBuilder ConfigureWebApplication(Action<WebApplication> configureDelegate)
    {
        _configureWebApplicationDelegate = configureDelegate;
        return this;
    }

    public override Application Build()
    {
        var app = _builder.Build();
        app.MapControllers();
        app.Urls.Add($"{Configure.Instance.HttpServerOption.Protocol}://{Configure.Instance.HttpServerOption.Host}:{Configure.Instance.HttpServerOption.Port}");

        if (Configure.Instance.BaseOption.IsDevelopment)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            EasyLogger.Info($"Swagger {Configure.Instance.HttpServerOption.Protocol}://{Configure.Instance.HttpServerOption.Host}:{Configure.Instance.HttpServerOption.Port}/swagger/index.html");
        }

        _configureWebApplicationDelegate?.Invoke(app);
        return new Application(app);
    }
}
