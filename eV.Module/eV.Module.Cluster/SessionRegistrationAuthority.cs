// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Cluster.Interface;
using eV.Module.EasyLog;
using StackExchange.Redis;
namespace eV.Module.Cluster;

public class SessionRegistrationAuthority : ISessionRegistrationAuthority
{
    // private readonly string _clusterName;
    // private readonly ConnectionMultiplexer _redis;
    // private const string Key = "eV:Cluster:{0}:SessionId:{1}";
    //
    // public SessionRegistrationAuthority(string clusterName, ConfigurationOptions redisOption)
    // {
    //     _clusterName = clusterName;
    //
    //     ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(redisOption);
    //     if (!conn.IsConnected)
    //         Logger.Error($"Cluster [{_clusterName}] session registration authority redis connection error");
    //     _redis = conn;
    // }
    // public void Registry(string nodeName, string sessionId)
    // {
    //     _redis.GetDatabase().StringSet(string.Format(Key, _clusterName, sessionId), nodeName);
    //     _redis.GetDatabase().HashSet(nodeName, sessionId, sessionId);
    // }
    // public void Deregister(string nodeName, string sessionId)
    // {
    //     _redis.GetDatabase().KeyDelete(string.Format(Key, _clusterName, sessionId));
    //     _redis.GetDatabase().HashDelete(nodeName, sessionId);
    // }
    // public string GetNodeName(string sessionId)
    // {
    //     var result = _redis.GetDatabase().StringGet(string.Format(Key, _clusterName, sessionId));
    //     return result.HasValue ? result.ToString() : "";
    // }
    // public List<string> GetSessionIdList(string nodeName)
    // {
    //     throw new NotImplementedException();
    // }
}
