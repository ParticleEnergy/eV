// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Cluster;
using eV.Module.Queue;
using eV.Module.Storage.Redis;
using StackExchange.Redis;

namespace eV.Framework.Server.Base;

public class ServiceBase
{
    private readonly RedisManager _redis = RedisManager.Instance;
    protected string NodeId => CommunicationManager.Instance == null ? string.Empty : CommunicationManager.Instance.NodeId;

    public IDatabase? GetRedis(string name)
    {
        if (!new List<string> { RedisNameReservedWord.QueueInstance, RedisNameReservedWord.ClusterInstance }.Contains(name)) return _redis.GetRedis(name);

        Module.EasyLog.Logger.Warn($"{name} is a reserved word");
        return null;
    }

    public async Task<bool> Produce<TValue>(TValue data)
    {
        if (MessageProcessor.Instance == null)
            return false;
        return await MessageProcessor.Instance.Produce(data);
    }

    public async Task<bool> SendInternalMessage<TValue>(string nodeId, TValue data)
    {
        if (CommunicationManager.Instance == null)
            return false;
        return await CommunicationManager.Instance.Produce(nodeId, data);
    }

    public async Task<bool> SendInternalMessage<TValue>(TValue data)
    {
        if (CommunicationManager.Instance == null)
            return false;
        return await CommunicationManager.Instance.Produce(data);
    }
}
