// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.Module.Session;
namespace eV.Framework.Server;

public class SessionDispatch
{

    private SessionDispatch()
    {
        SessionGroup = new SessionGroup();
        SessionManager = new SessionManager();
    }
    public static SessionDispatch Instance { get; } = new();
    public SessionGroup SessionGroup { get; }
    public SessionManager SessionManager { get; }
}
