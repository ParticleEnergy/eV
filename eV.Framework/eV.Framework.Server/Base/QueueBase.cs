// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Module.Queue.Interface;
using EasyLogger = eV.Module.EasyLog.Logger;

namespace eV.Framework.Server.Base;

public abstract class QueueBase : IQueueHandler
{
    public string QueueName { get; set; } = string.Empty;

    public void RunConsume()
    {
        try
        {
            new Task(Consume).Start();
        }
        catch (Exception e)
        {
            EasyLogger.Error(e.Message, e);
        }
    }

    protected abstract void Consume();
}
