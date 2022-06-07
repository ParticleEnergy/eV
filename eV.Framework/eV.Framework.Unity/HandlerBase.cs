// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Module.Routing.Interface;
namespace eV.Framework.Unity;

public abstract class HandlerBase<TContent> : IHandler
{
    public virtual Task Run(ISession session, object content)
    {
        Handle(session, (TContent)content);
        return Task.CompletedTask;
    }
    protected abstract void Handle(ISession session, TContent content);
}
