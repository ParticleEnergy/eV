// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Concurrent;

namespace eV.Module.Routing.Interface;

public delegate void SessionEvent(ISession session);

public interface ISession
{
    public string? SessionId { get; set; }
    public Hashtable SessionData { get; }
    public DateTime? ConnectedDateTime { get; set; }
    public DateTime? LastActiveDateTime { get; set; }
    public bool Send(byte[] data);
    public bool Send<T>(T data);
    public bool Send<T>(string groupId, T data);
    public void SendGroup<T>(string groupId, T data);
    public void SendBroadcast<T>(T data);
    public bool JoinGroup(string groupId);
    public bool LeaveGroup(string groupId);
    public ConcurrentDictionary<string, string>? GetGroup(string groupId);
    public bool CreateGroup(string groupId);
    public bool DeleteGroup(string groupId);
    public void Activate(string sessionId);
    public void Shutdown();
}
