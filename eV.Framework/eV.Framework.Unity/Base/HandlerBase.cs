// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Module.Routing.Interface;

namespace eV.Framework.Unity.Base;

public abstract class HandlerBase<TContent> : IHandler
{
    public virtual async Task Run(ISession session, object content)
    {
        await Handle(session, (TContent)content);
    }

    protected abstract Task Handle(ISession session, TContent content);
}
