// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using eV.EasyLog;
namespace eV.Session
{
    public class SessionGroup
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _allGroup = new();

        public ConcurrentDictionary<string, string>? GetGroup(string groupName)
        {
            return _allGroup.TryGetValue(groupName, out ConcurrentDictionary<string, string>? result) ? result : null;
        }

        public void CreateGroup(string groupName)
        {
            Logger.Info($"Create group {groupName}");
            _allGroup[groupName] = new ConcurrentDictionary<string, string>();
        }

        public bool DeleteGroup(string groupName)
        {
            Logger.Info($"Delete group {groupName}");
            return _allGroup.TryRemove(groupName, out ConcurrentDictionary<string, string>? _);
        }

        public bool JoinGroup(string groupName, string sessionId)
        {
            if (!_allGroup.ContainsKey(groupName))
                return false;

            if (!_allGroup.TryGetValue(groupName, out ConcurrentDictionary<string, string>? group))
                return false;

            Logger.Info($"Session {sessionId} join group {groupName}");
            group[sessionId] = sessionId;
            return true;
        }

        public bool LeaveGroup(string groupName, string sessionId)
        {
            if (!_allGroup.ContainsKey(groupName))
                return false;

            if (!_allGroup.TryGetValue(groupName, out ConcurrentDictionary<string, string>? group))
                return false;

            Logger.Info($"Session {sessionId} Leave group {groupName}");
            group.TryRemove(sessionId, out string? _);
            return true;
        }
    }
}
