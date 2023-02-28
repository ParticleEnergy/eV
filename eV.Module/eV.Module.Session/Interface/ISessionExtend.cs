// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Routing.Interface;

namespace eV.Module.Session.Interface;

public interface ISessionExtend
{
    public Task<bool> Send(string sessionId, byte[] data);
    public Task SendBroadcast(string selfSessionId, byte[] data);
    public Task OnActivate(ISession session);
    public Task OnRelease(ISession session);
}
