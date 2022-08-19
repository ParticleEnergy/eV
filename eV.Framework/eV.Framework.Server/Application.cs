// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Framework.Server.Logger;
using eV.Module.GameProfile;
using eV.Module.Routing.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using EasyLogger = eV.Module.EasyLog.Logger;
namespace eV.Framework.Server;

public class Application
{
    private string[]? _args;
    private readonly List<Type> _singletons = new();
    private bool _isOnlyTcp;

    public Application()
    {
        EasyLogger.SetLogger(new ServerLog(Configure.Instance.ProjectName));
        EasyLogger.Info(DefaultSetting.Logo);
    }
    #region
    public Application SetArgs(string[] args)
    {
        _args = args;
        return this;
    }

    public Application AddSingleton(Type singleton)
    {
        _singletons.Add(singleton);
        return this;
    }

    public Application OnlyTcp()
    {
        _isOnlyTcp = true;
        return this;
    }
    #endregion

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

    public void Run()
    {
        (_isOnlyTcp ? GetOnlyTcpApp() : GetApp()).Run();
    }

    public async Task RunAsync()
    {
        await (_isOnlyTcp ? GetOnlyTcpApp() : GetApp()).RunAsync();
    }

    private IHost GetApp()
    {
        var webApplicationBuilder = _args == null ? WebApplication.CreateBuilder() : WebApplication.CreateBuilder(_args);
        foreach (Type singleton in _singletons)
        {
            webApplicationBuilder.Services.AddSingleton(singleton);
        }
        webApplicationBuilder.Services.AddSingleton<IHostedService, TcpHostedService>();
        webApplicationBuilder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddProvider(new ServerLoggerProvider());
        });
        webApplicationBuilder.Services.AddControllers();
        webApplicationBuilder.Services.AddEndpointsApiExplorer();
        webApplicationBuilder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = Configure.Instance.ProjectName
            });
        });

        var app = webApplicationBuilder.Build();
        app.MapControllers();
        app.Urls.Add($"http://{Configure.Instance.HttpServerOption.Host}:{Configure.Instance.HttpServerOption.Port}");

        if (!Configure.Instance.BaseOption.IsDevelopment)
            return app;

        app.UseSwagger();
        app.UseSwaggerUI();
        EasyLogger.Info($"Swagger http://{Configure.Instance.HttpServerOption.Host}:{Configure.Instance.HttpServerOption.Port}/swagger/index.html");
        return app;
    }

    private IHost GetOnlyTcpApp()
    {
        return (_args == null ? Host.CreateDefaultBuilder() : Host.CreateDefaultBuilder(_args)).ConfigureServices(services =>
            {
                foreach (Type singleton in _singletons)
                {
                    services.AddSingleton(singleton);
                }
                services.AddSingleton<IHostedService, TcpHostedService>();
                services.AddLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddProvider(new ServerLoggerProvider());
                });
            })
            .Build();
    }
}
