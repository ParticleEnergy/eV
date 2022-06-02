// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Net;
using Confluent.Kafka;
using eV.Framework.Server.Options;
using StackExchange.Redis;
namespace eV.Framework.Server;

public static class Utils
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
        if (option.Keepalive != null)
            config.KeepAlive = (int)option.Keepalive;
        if (option.Address.Length == 1 && option.Database != null)
            config.DefaultDatabase = option.Database;

        config.DefaultVersion = new Version(4, 0, 9);
        return config;
    }

    public static KeyValuePair<ProducerConfig, ConsumerConfig> GetKafkaConfig(KafkaOption option)
    {
        ProducerConfig producerConfig = new()
        {
            BootstrapServers = option.Address,
            ClientId = Dns.GetHostName(),
            SocketKeepaliveEnable = true

        };
        ConsumerConfig consumerConfig = new()
        {
            BootstrapServers = option.Address,
            GroupId = Configure.Instance.ProjectName,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true,
            EnablePartitionEof = true,
            SocketKeepaliveEnable = true
        };
        if (!option.SaslMechanism.Equals(""))
        {
            switch (option.SaslMechanism)
            {
                case "Gssapi":
                    producerConfig.SaslMechanism = SaslMechanism.Gssapi;
                    producerConfig.SaslMechanism = SaslMechanism.Gssapi;
                    break;
                case "Plain":
                    producerConfig.SaslMechanism = SaslMechanism.Plain;
                    consumerConfig.SaslMechanism = SaslMechanism.Plain;
                    break;
                case "ScramSha256":
                    producerConfig.SaslMechanism = SaslMechanism.ScramSha256;
                    consumerConfig.SaslMechanism = SaslMechanism.ScramSha256;
                    break;
                case "ScramSha512":
                    producerConfig.SaslMechanism = SaslMechanism.ScramSha512;
                    consumerConfig.SaslMechanism = SaslMechanism.ScramSha512;
                    break;
                case "OAuthBearer":
                    producerConfig.SaslMechanism = SaslMechanism.OAuthBearer;
                    consumerConfig.SaslMechanism = SaslMechanism.OAuthBearer;
                    break;
                default:
                    producerConfig.SaslMechanism = SaslMechanism.Plain;
                    consumerConfig.SaslMechanism = SaslMechanism.Plain;
                    break;
            }
        }
        if (!option.SecurityProtocol.Equals(""))
        {
            switch (option.SecurityProtocol)
            {
                case "Plaintext":
                    producerConfig.SecurityProtocol = SecurityProtocol.Plaintext;
                    producerConfig.SecurityProtocol = SecurityProtocol.Plaintext;
                    break;
                case "Ssl":
                    producerConfig.SecurityProtocol = SecurityProtocol.Ssl;
                    consumerConfig.SecurityProtocol = SecurityProtocol.Ssl;
                    break;
                case "SaslPlaintext":
                    producerConfig.SecurityProtocol = SecurityProtocol.SaslPlaintext;
                    consumerConfig.SecurityProtocol = SecurityProtocol.SaslPlaintext;
                    break;
                case "SaslSsl":
                    producerConfig.SecurityProtocol = SecurityProtocol.SaslSsl;
                    consumerConfig.SecurityProtocol = SecurityProtocol.SaslSsl;
                    break;
                default:
                    producerConfig.SecurityProtocol = SecurityProtocol.SaslSsl;
                    consumerConfig.SecurityProtocol = SecurityProtocol.SaslSsl;
                    break;
            }
        }
        if (!option.SaslUsername.Equals(""))
        {
            producerConfig.SaslUsername = option.SaslUsername;
            consumerConfig.SaslUsername = option.SaslUsername;
        }
        if (!option.SaslPassword.Equals(""))
        {
            producerConfig.SaslPassword = option.SaslPassword;
            consumerConfig.SaslPassword = option.SaslPassword;
        }
        if (option.SocketTimeoutMs > 0)
        {
            producerConfig.SocketTimeoutMs = option.SocketTimeoutMs;
            consumerConfig.SocketTimeoutMs = option.SocketTimeoutMs;
        }
        if (option.SocketReceiveBufferBytes > 0)
        {
            producerConfig.SocketReceiveBufferBytes = option.SocketReceiveBufferBytes;
            consumerConfig.SocketReceiveBufferBytes = option.SocketReceiveBufferBytes;
        }
        if (option.SocketSendBufferBytes > 0)
        {
            producerConfig.SocketSendBufferBytes = option.SocketSendBufferBytes;
            consumerConfig.SocketSendBufferBytes = option.SocketSendBufferBytes;
        }

        if (option.HeartbeatIntervalMs > 0)
        {
            consumerConfig.HeartbeatIntervalMs = option.HeartbeatIntervalMs;
        }
        if (option.SessionTimeoutMs > 0)
        {
            consumerConfig.SessionTimeoutMs = option.SessionTimeoutMs;
        }
        return KeyValuePair.Create(producerConfig, consumerConfig);
    }
}
