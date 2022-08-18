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
    private readonly IHost _app;

    public Application(string[] args, bool onlyTcp = false)
    {
        EasyLogger.SetLogger(new ServerLog(Configure.Instance.ProjectName));
        EasyLogger.Info(DefaultSetting.Logo);

        _app = onlyTcp ? GetOnlyTcpApp(args) : GetApp(args);
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
        _app.Run();
    }

    public async Task RunAsync()
    {
        await _app.RunAsync();
    }

    public void Start()
    {
        _app.Start();
    }

    public async Task StartAsync()
    {
        await _app.StartAsync();
    }

    public async Task StopAsync()
    {
        await _app.StopAsync();
    }

    private IHost GetApp(string[] args)
    {
        var webApplicationBuilder = WebApplication.CreateBuilder(args);
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

    private IHost GetOnlyTcpApp(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
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
