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
    private const string TcpServerOptionKey = "TcpServer";
    private const string HttpServerOptionKey = "HttpServer";
    private const string ClusterOptionKey = "Cluster";
    private const string MongodbOptionKey = "Mongodb";
    private const string RedisOptionKey = "Redis";
    private const string KafkaOptionKey = "Kafka";

    private Configure()
    {
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);

        ConfigurationBuilder builder = new();
        builder.AddJsonFile(attributes.Length == 0 ? "appsettings.json" : ((AssemblyConfigurationAttribute)attributes[0]).Configuration);
        Config = builder.Build();

        ProjectName = Config.GetSection(ProjectNameKey).Value;
        BaseOption = Config.GetSection(BaseOptionKey).Get<BaseOption>();
        TcpServerOption = Config.GetSection(TcpServerOptionKey).Get<TcpServerOption>();
        HttpServerOption = Config.GetSection(HttpServerOptionKey).Get<HttpServerOption>();
        MongodbOption = Config.GetSection(MongodbOptionKey).Get<Dictionary<string, string>>();
        RedisOption = Config.GetSection(RedisOptionKey).Get<Dictionary<string, RedisOption>>();
        KafkaOption = Config.GetSection(KafkaOptionKey).Get<Dictionary<string, KafkaOption>>();
        ClusterOption = Config.GetSection(ClusterOptionKey).Get<ClusterOption>();
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
    public string ProjectName { get; }
    public BaseOption BaseOption { get; }
    public TcpServerOption TcpServerOption { get; }
    public HttpServerOption HttpServerOption { get; }
    public Dictionary<string, string>? MongodbOption { get; }
    public Dictionary<string, RedisOption>? RedisOption { get; }
    public Dictionary<string, KafkaOption>? KafkaOption { get; }
    public ClusterOption? ClusterOption { get; }
    #endregion
}
