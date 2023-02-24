// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Collections;

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
    public bool Send<T>(string sessionId, T data);
    public void SendBroadcast<T>(T data);
    public void Activate(string sessionId);
    public void Shutdown();
}
