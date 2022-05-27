// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Reflection;
using eV.Module.Job.Attributes;
using eV.Module.Job.Interface;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using Logger = eV.Module.EasyLog.Logger;
namespace eV.Module.Job;

public class Job
{
    private readonly string _nsName;
    private readonly Task<IScheduler> _scheduler;
    public Job(string nsName)
    {
        LogProvider.SetCurrentLogProvider(new Log());
        StdSchedulerFactory factory = new();
        _scheduler = factory.GetScheduler();

        _scheduler.Result.Start();

        _nsName = nsName;
    }

    public Job(string nsName, Task<IScheduler> scheduler)
    {
        LogProvider.SetCurrentLogProvider(new Log());

        _scheduler = scheduler;

        _scheduler.Result.Start();

        _nsName = nsName;
    }

    private void RegisterHandler()
    {
        Type[] allTypes = Assembly.Load(_nsName).GetExportedTypes();

        foreach (Type type in allTypes)
        {
            object[] jobAttributes = type.GetCustomAttributes(typeof(JobAttribute), true);

            if (jobAttributes.Length <= 0)
                continue;

            if (Activator.CreateInstance(type) is not IJobHandler handler)
                continue;

            _scheduler.Result.ScheduleJob(handler.Job, handler.Trigger);

            Logger.Info($"JobHandler [{type.FullName}] JobName: {handler.JobName} TriggerName: {handler.TriggerName} GroupName:{handler.GroupName} registration succeeded");
        }
    }

    public void Start()
    {
        RegisterHandler();
    }

    public async void Stop()
    {
        await _scheduler.Result.Shutdown();
    }
}
