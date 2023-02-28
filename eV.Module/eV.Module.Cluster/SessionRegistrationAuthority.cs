// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Cluster.Interface;
using StackExchange.Redis;

namespace eV.Module.Cluster;

public class SessionRegistrationAuthority : ISessionRegistrationAuthority
{
    private readonly string _clusterId;
    private readonly string _nodeId;

    private readonly ConnectionMultiplexer _redis;

    private const string HashTableKey = "eV:Cluster:{0}";
    private const string SetKey = "eV:Cluster:{0}:node{1}";

    public SessionRegistrationAuthority(string clusterId, string nodeId, ConnectionMultiplexer redis)
    {
        _clusterId = clusterId;
        _nodeId = nodeId;
        _redis = redis;
    }

    public void Registry(string sessionId)
    {
        _redis.GetDatabase().HashSet(string.Format(HashTableKey, _clusterId), sessionId, _nodeId);
        _redis.GetDatabase().SetAdd(string.Format(SetKey, _clusterId, _nodeId), sessionId);
    }

    public void Deregister(string sessionId)
    {
        _redis.GetDatabase().HashDelete(string.Format(HashTableKey, _clusterId), sessionId);
        _redis.GetDatabase().SetRemove(string.Format(SetKey, _clusterId, _nodeId), sessionId);
    }

    public async Task<List<string>> GetAllNodeIds()
    {
        List<string> result = new();
        result.AddRange(from redisValue in await _redis.GetDatabase().HashKeysAsync(string.Format(HashTableKey, _clusterId)) select redisValue.ToString());
        return result;
    }

    public async Task<string> GetNodeId(string sessionId)
    {
        var result = await _redis.GetDatabase().HashGetAsync(string.Format(HashTableKey, _clusterId), sessionId);
        return result.ToString();
    }

    public async Task<List<string>> GetAllSessionIds()
    {
        List<string> result = new();

        result.AddRange(from hashEntry in await _redis.GetDatabase().HashGetAllAsync(string.Format(HashTableKey, _clusterId)) select hashEntry.Name.ToString());
        return result;
    }

    public async Task<List<string>> GetSessionIdsByNode(string nodeId)
    {
        List<string> result = new();
        result.AddRange(from value in await _redis.GetDatabase().SetMembersAsync(string.Format(SetKey, _clusterId, nodeId)) select value.ToString());
        return result;
    }

    public void Start()
    {
        if (_redis.GetDatabase().KeyExists(string.Format(HashTableKey, _clusterId))) return;
        _redis.GetDatabase().HashSet(string.Format(HashTableKey, _clusterId), Array.Empty<HashEntry>());
    }

    public async void Stop()
    {
        var batch = _redis.GetDatabase().CreateBatch();
        foreach (string sessionId in await GetSessionIdsByNode(_nodeId))
        {
            await batch.HashDeleteAsync(string.Format(HashTableKey, _clusterId), sessionId);
        }

        await batch.KeyDeleteAsync(string.Format(SetKey, _clusterId, _nodeId));
        batch.Execute();
    }
}
