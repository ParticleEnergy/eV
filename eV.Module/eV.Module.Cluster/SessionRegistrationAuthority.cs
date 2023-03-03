// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Cluster.Interface;
using StackExchange.Redis;

namespace eV.Module.Cluster;

public class SessionRegistrationAuthority : ISessionRegistrationAuthority
{
    private readonly string _clusterId;
    private readonly string _nodeId;

    private readonly IDatabase _redisInstance;

    private const string HashTableKey = "eV:Cluster:{0}";
    private const string SetKey = "eV:Cluster:{0}:node:{1}";

    public SessionRegistrationAuthority(string clusterId, string nodeId, IConnectionMultiplexer redis)
    {
        _clusterId = clusterId;
        _nodeId = nodeId;
        _redisInstance = redis.GetDatabase();
    }

    public void Registry(string sessionId)
    {
        _redisInstance.HashSet(string.Format(HashTableKey, _clusterId), sessionId, _nodeId);
        _redisInstance.SetAdd(string.Format(SetKey, _clusterId, _nodeId), sessionId);
    }

    public void Deregister(string sessionId)
    {
        _redisInstance.HashDelete(string.Format(HashTableKey, _clusterId), sessionId);
        _redisInstance.SetRemove(string.Format(SetKey, _clusterId, _nodeId), sessionId);
    }

    public async Task<List<string>> GetAllNodeIds()
    {
        List<string> result = new();
        result.AddRange(from redisValue in await _redisInstance.HashKeysAsync(string.Format(HashTableKey, _clusterId)) select redisValue.ToString());
        return result;
    }

    public async Task<string> GetNodeId(string sessionId)
    {
        var result = await _redisInstance.HashGetAsync(string.Format(HashTableKey, _clusterId), sessionId);
        return result.ToString();
    }

    public async Task<List<string>> GetAllSessionIds()
    {
        List<string> result = new();

        result.AddRange(from hashEntry in await _redisInstance.HashGetAllAsync(string.Format(HashTableKey, _clusterId)) select hashEntry.Name.ToString());
        return result;
    }

    public async Task<List<string>> GetSessionIdsByNode(string nodeId)
    {
        List<string> result = new();
        result.AddRange(from value in await _redisInstance.SetMembersAsync(string.Format(SetKey, _clusterId, nodeId)) select value.ToString());
        return result;
    }

    public void Start()
    {
        if (_redisInstance.KeyExists(string.Format(HashTableKey, _clusterId))) return;
        _redisInstance.HashSet(string.Format(HashTableKey, _clusterId), Array.Empty<HashEntry>());
    }

    public async void Stop()
    {
        var batch = _redisInstance.CreateBatch();
        foreach (string sessionId in await GetSessionIdsByNode(_nodeId))
        {
            await batch.HashDeleteAsync(string.Format(HashTableKey, _clusterId), sessionId);
        }

        await batch.KeyDeleteAsync(string.Format(SetKey, _clusterId, _nodeId));
        batch.Execute();
    }
}
