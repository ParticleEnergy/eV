// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Reflection;
using eV.Framework.Server.Options;
using Microsoft.Extensions.Configuration;

namespace eV.Framework.Server;

public class Configure
{
    private const string ProjectNameKey = "ProjectName";
    private const string BaseOptionKey = "Base";
    private const string ServerOptionKey = "Server";
    private const string MongodbOptionKey = "Mongodb";
    private const string RedisOptionKey = "Redis";

    private Configure()
    {
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);

        ConfigurationBuilder builder = new();
        builder.AddJsonFile(attributes.Length == 0 ? "appsettings.json" : ((AssemblyConfigurationAttribute)attributes[0]).Configuration);
        Config = builder.Build();
    }

    public IConfiguration Config { get; }
    public static Configure Instance { get; } = new();

    #region Options

    public string ProjectName => Config.GetSection(ProjectNameKey).Value;

    public BaseOption BaseOption
    {
        get
        {
            var config = Config.GetSection(BaseOptionKey).Get<BaseOption>();
            if (config == null)
            {
                throw new Exception("The \"Base\" field is missing from the appsettings.json file");
            }

            return config;
        }
    }

    public ServerOption ServerOption
    {
        get
        {
            var config = Config.GetSection(ServerOptionKey).Get<ServerOption>();
            if (config == null)
            {
                throw new Exception("The \"Server\" field is missing from the appsettings.json file");
            }

            return config;
        }
    }

    public Dictionary<string, MongoOption>? MongodbOption =>
        Config.GetSection(MongodbOptionKey).Get<Dictionary<string, MongoOption>>();

    public Dictionary<string, RedisOption>? RedisOption =>
        Config.GetSection(RedisOptionKey).Get<Dictionary<string, RedisOption>>();

    #endregion
}
