// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Framework.Server.Logger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace eV.Framework.Server;

public static class ApplicationBuilder
{
    public static TcpBuilder CreateTcpBuilder(string[] args)
    {
        return new TcpBuilder(args);
    }

    public static WebApplicationBuilder CreateTcpAndWebBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddLogging(
            loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddProvider(new ServerLoggerProvider());
            }
        );
        builder.Services.AddSingleton<IHostedService, TcpHostedService>();
        return builder;
    }
}
