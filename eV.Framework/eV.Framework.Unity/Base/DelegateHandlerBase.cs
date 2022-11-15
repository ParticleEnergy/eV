// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Routing.Interface;

namespace eV.Framework.Unity.Base;

public abstract class DelegateHandlerBase<TContent> : IHandler
{
    public event Handler<TContent>? Handler;

    public virtual Task Run(ISession session, object content)
    {
        Handler?.Invoke(session, (TContent)content);
        return Task.CompletedTask;
    }
}
