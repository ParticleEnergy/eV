// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using eV.Module.EasyLog;

namespace eV.Module.Session;

public class SessionGroup
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _allGroup = new();

    public ConcurrentDictionary<string, string>? GetGroup(string groupId)
    {
        return _allGroup.TryGetValue(groupId, out ConcurrentDictionary<string, string>? result) ? result : null;
    }

    public bool CreateGroup(string groupId)
    {
        _allGroup.TryGetValue(groupId, out ConcurrentDictionary<string, string>? group);
        if (group == null)
        {
            Logger.Info($"Create group {groupId}");
            _allGroup[groupId] = new ConcurrentDictionary<string, string>();
            return true;
        }
        Logger.Error($"Group {groupId} is already exists");
        return false;
    }

    public bool DeleteGroup(string groupId)
    {
        Logger.Info($"Delete group {groupId}");
        return _allGroup.TryRemove(groupId, out ConcurrentDictionary<string, string>? _);
    }

    public bool JoinGroup(string groupId, string sessionId)
    {
        if (!_allGroup.ContainsKey(groupId))
            return false;

        if (!_allGroup.TryGetValue(groupId, out ConcurrentDictionary<string, string>? group))
            return false;

        Logger.Info($"Session {sessionId} join group {groupId}");
        group[sessionId] = sessionId;
        return true;
    }

    public bool LeaveGroup(string groupId, string sessionId)
    {
        if (!_allGroup.ContainsKey(groupId))
            return false;

        if (!_allGroup.TryGetValue(groupId, out ConcurrentDictionary<string, string>? group))
            return false;

        Logger.Info($"Session {sessionId} Leave group {groupId}");
        return group.TryRemove(sessionId, out string? _);
    }
}
