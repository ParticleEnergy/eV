// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.Routing.Interface;
using log4net;
namespace eV.Server.Base
{
    public abstract class HandlerBase<TContent> : IHandler
    {
        protected readonly ILog Logger = LogManager.GetLogger(DefaultSetting.LoggerName);

        protected abstract void Handle(ISession session, TContent content);

        public void Run(ISession session, object content)
        {
            Handle(session, (TContent)content);
        }
    }
}
