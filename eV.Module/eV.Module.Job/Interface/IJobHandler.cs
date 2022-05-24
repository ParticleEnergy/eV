// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using Quartz;
namespace eV.Module.Job.Interface;

public interface IJobHandler
{
    public string JobName { get; }
    public string TriggerName { get; }
    public string GroupName { get; }
    public IJobDetail Job { get; }
    public ITrigger Trigger { get; }
}
