// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using System.Security.Authentication;
using eV.Framework.Server.Options;
using StackExchange.Redis;

namespace eV.Framework.Server.Utils;

public static class ConfigUtils
{
    public static ConfigurationOptions GetRedisConfig(RedisOption option)
    {
        ConfigurationOptions config = new();
        foreach (string[] address in option.Address.Select(address => address.Split(":")))
        {
            config.EndPoints.Add(address[0], Convert.ToInt32(address[1]));
        }

        if (option.User != null)
            config.User = option.User;
        if (option.Password != null)
            config.Password = option.Password;
        if (option.Address.Length == 1 && option.Database != null)
            config.DefaultDatabase = option.Database;
        if (option.Keepalive != null)
            config.KeepAlive = (int)option.Keepalive;
        if (option.AsyncTimeout != null)
            config.AsyncTimeout = (int)option.AsyncTimeout;
        if (option.SyncTimeout != null)
            config.SyncTimeout = (int)option.SyncTimeout;

        if (option.Ssl != null && (bool)option.Ssl && option.SslHost != null && !option.SslHost.Equals(""))
        {
            config.Ssl = true;
            config.SslHost = option.SslHost;
            config.SslProtocols = SslProtocols.Tls13;
        }

        if (option.AllowAdmin != null)
            config.AllowAdmin = (bool)option.AllowAdmin;

        config.DefaultVersion = new Version(option.VersionMajor, option.VersionMinor, option.VersionBuild);
        return config;
    }
}
