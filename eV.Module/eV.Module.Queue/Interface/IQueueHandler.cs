// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Module.Queue.Interface;

public interface IQueueHandler
{
    public string QueueName { get; set; }
    public void RunConsume();
}
