// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Cluster.Interface;
using eV.Module.EasyLog;
using StackExchange.Redis;
namespace eV.Module.Cluster;

public class SessionRegistrationAuthority : ISessionRegistrationAuthority
{
    private readonly string _clusterName;
    private readonly string _nodeName;
    private readonly ConnectionMultiplexer _redis;

    private const string SessionKey = "eV:Cluster:{0}:SessionId:{1}";
    private const string NodeKey = "eV:Cluster:{0}:Node{1}";
    public SessionRegistrationAuthority(string clusterName, string nodeName, ConfigurationOptions redisOption)
    {
        _clusterName = clusterName;
        _nodeName = nodeName;
        ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(redisOption);
        if (!conn.IsConnected)
            Logger.Error($"Cluster [{_clusterName}] session registration authority redis connection error");
        _redis = conn;
    }

    public void Registry(string sessionId)
    {
        _redis.GetDatabase().StringSet(string.Format(SessionKey, _clusterName, sessionId), _nodeName);
        _redis.GetDatabase().HashSet(string.Format(NodeKey, _clusterName, _nodeName), sessionId, sessionId);
    }
    public void Deregister(string sessionId)
    {
        _redis.GetDatabase().KeyDelete(string.Format(SessionKey, _clusterName, sessionId));
        _redis.GetDatabase().HashDelete(string.Format(NodeKey, _clusterName, _nodeName), sessionId);
    }
    public string GetNodeName(string sessionId)
    {
        var result = _redis.GetDatabase().StringGet(string.Format(SessionKey, _clusterName, sessionId));
        return result.HasValue ? result.ToString() : "";
    }
    public List<string> GetAllSessionId()
    {
        var resultHash = _redis.GetDatabase().HashGetAll(string.Format(NodeKey, _clusterName, _nodeName));
        return resultHash.Select(rh => rh.Value).Select(dummy => (string)dummy).ToList();
    }
}
