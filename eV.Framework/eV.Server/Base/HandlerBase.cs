// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.


using eV.Routing.Interface;
namespace eV.Server.Base;

public abstract class HandlerBase<TContent> : IHandler
{
    protected bool Skip = false;

    public virtual void Run(ISession session, object content)
    {
        if (!Skip && session.SessionId is null or "")
            return;
        Handle(session, (TContent)content);
    }
    protected abstract void Handle(ISession session, TContent content);
}
