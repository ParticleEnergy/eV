// See https://aka.ms/new-console-template for more information


using eV.ServerExample.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IHostedService, LifetimeEventsHostedService>();
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddProvider(new LoggerProvider());
        });
    })
    .Build();

await host.RunAsync();


//
// using eV.Framework.Server;
// using eV.Module.GameProfile;
// using eV.Module.Routing.Interface;
// using eV.ServerExample;
// Profile.OnLoad += delegate
// {
//     Profile.AssignmentConfigObject(GameProfile.Instance);
// };
// Server server = new();
// server.OnConnected += delegate(ISession session)
// {
//
// };
// server.SessionOnActivate += delegate(ISession session)
// {
//
// };
// server.SessionOnRelease += delegate(ISession session)
// {
//
// };
// server.Start();
//
// Console.ReadLine();
