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
    private readonly ConfigurationOptions _redisOption;

    private const string SessionKey = "eV:Cluster:{0}:SessionId:{1}";
    private const string NodeKey = "eV:Cluster:{0}:Node{1}";

    private ConnectionMultiplexer? _redis;

    public SessionRegistrationAuthority(string clusterName, string nodeName, ConfigurationOptions redisOption)
    {
        _clusterName = clusterName;
        _nodeName = nodeName;
        _redisOption = redisOption;
    }

    public void Registry(string sessionId)
    {
        _redis?.GetDatabase().StringSet(string.Format(SessionKey, _clusterName, sessionId), _nodeName);
        _redis?.GetDatabase().HashSet(string.Format(NodeKey, _clusterName, _nodeName), sessionId, sessionId);
    }
    public void Deregister(string sessionId)
    {
        _redis?.GetDatabase().KeyDelete(string.Format(SessionKey, _clusterName, sessionId));
        _redis?.GetDatabase().HashDelete(string.Format(NodeKey, _clusterName, _nodeName), sessionId);
    }
    public string GetNodeName(string sessionId)
    {
        var result = _redis?.GetDatabase().StringGet(string.Format(SessionKey, _clusterName, sessionId));
        return (result == "" ? "" : result.HasValue ? result.ToString() : "") ?? string.Empty;
    }
    public List<string> GetAllSessionId()
    {
        var resultHash = _redis?.GetDatabase().HashGetAll(string.Format(NodeKey, _clusterName, _nodeName));

        return resultHash == null ? new List<string>() : resultHash.Select(rh => rh.Value).Select(dummy => (string)dummy).ToList();
    }

    public void Start()
    {
        ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(_redisOption);
        if (!conn.IsConnected)
            Logger.Error($"Cluster [{_clusterName}] session registration authority redis connection error");
        _redis = conn;
    }

    public void Stop()
    {
        _redis?.Close();
    }
}
