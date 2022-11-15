// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Routing.Interface;

namespace eV.Framework.Server.Interface;

public interface ISessionDrive
{
    public bool Send(string sessionId, byte[] data);
    public void SendGroup(string selfSessionId, string groupId, byte[] data);
    public void SendBroadcast(string selfSessionId, byte[] data);
    public bool Activate(ISession session);
    public bool Release(ISession session);
}
