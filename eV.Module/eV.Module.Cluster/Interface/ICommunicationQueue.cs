// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Module.Cluster.Interface;

public interface ICommunicationQueue
{
    public event Func<string, byte[], bool>? SendAction;
    public event Action<string, byte[]>? SendGroupAction;
    public event Action<byte[]>? SendBroadcastAction;
    public void Send(string sessionId, byte[] data);
    public void SendGroup(string groupId, byte[] data);
    public void SendBroadcast(byte[] data);
    public void Start();
    public void Stop();
}
