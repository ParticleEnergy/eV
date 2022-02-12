// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Reflection;
using eV.Server.Options;
using Microsoft.Extensions.Configuration;
namespace eV.Server;

public class Configure
{

    private Configure()
    {
        ConfigurationBuilder builder = new();

        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
        builder.AddJsonFile(attributes.Length == 0 ? "appsettings.json" : ((AssemblyConfigurationAttribute)attributes[0]).Configuration);
        Config = builder.Build();

        BaseOptions = Config.GetSection(BaseOptions.Keyword).Get<BaseOptions>();
        ServerOptions = Config.GetSection(ServerOptions.Keyword).Get<ServerOptions>();
        StorageOptions = Config.GetSection(StorageOptions.Keyword).Get<StorageOptions>();
    }

    public IConfiguration Config
    {
        get;
    }
    public static Configure Instance
    {
        get;
    } = new();
    #region Options
    public BaseOptions BaseOptions { get; }
    public ServerOptions ServerOptions { get; }
    public StorageOptions StorageOptions { get; }
    #endregion
}
