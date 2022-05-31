// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Module.EasyLog;
using eV.Module.Queue.Interface;
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
            Logger.Error(e.Message, e);
        }
    }

    protected abstract void Consume();
}
