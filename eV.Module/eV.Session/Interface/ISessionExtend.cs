// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.Routing.Interface;
namespace eV.Session.Interface;

public interface ISessionExtend
{
    public bool Send(string sessionId, byte[] data);
    public void SendGroup(string selfSessionId, string groupId, byte[] data);
    public void SendBroadcast(string selfSessionId, byte[] data);
    public bool JoinGroup(string groupId, string sessionId);
    public bool LeaveGroup(string groupId, string sessionId);
    public void OnActivate(ISession session);
    public void OnRelease(ISession session);
}
