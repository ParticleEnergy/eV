// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Collections;
namespace eV.Routing.Interface;

public delegate void SessionEvent(ISession session);
public interface ISession
{
    public string? SessionId { get; set; }
    public Hashtable SessionData { get; }
    public Dictionary<string, string> Group { get; set; }
    public DateTime? ConnectedDateTime { get; set; }
    public DateTime? LastActiveDateTime { get; set; }
    public bool Send(byte[] data);
    public bool Send<T>(T data);
    public bool Send<T>(string groupId, T data);
    public void SendGroup<T>(string groupId, T data);
    public void SendBroadcast<T>(T data);
    public bool JoinGroup(string groupId);
    public bool LeaveGroup(string groupId);
    public void Activate(string sessionId);
    public void Shutdown();
}
