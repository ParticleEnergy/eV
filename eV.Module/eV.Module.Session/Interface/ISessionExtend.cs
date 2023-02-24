// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Routing.Interface;

namespace eV.Module.Session.Interface;

public interface ISessionExtend
{
    public bool Send(string sessionId, byte[] data);
    public void SendBroadcast(string selfSessionId, byte[] data);
    public void OnActivate(ISession session);
    public void OnRelease(ISession session);
}
