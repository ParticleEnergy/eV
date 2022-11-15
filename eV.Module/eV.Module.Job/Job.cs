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
    private string _assemblyString = string.Empty;
    private IScheduler? _scheduler;

    public Job(string assemblyString)
    {
        LogProvider.SetCurrentLogProvider(new Log());
        Init(assemblyString);
    }

    public Job(string assemblyString, IScheduler scheduler)
    {
        LogProvider.SetCurrentLogProvider(new Log());
        Init(assemblyString, scheduler);
    }

    private async void Init(string assemblyString, IScheduler? scheduler = null)
    {
        if (scheduler == null)
        {
            StdSchedulerFactory factory = new();
            _scheduler = await factory.GetScheduler();
        }
        else
        {
            _scheduler = scheduler;
        }

        await _scheduler.Start();
        _assemblyString = assemblyString;
    }

    private void RegisterHandler()
    {
        Type[] allTypes = Assembly.Load(_assemblyString).GetExportedTypes();

        foreach (Type type in allTypes)
        {
            object[] jobAttributes = type.GetCustomAttributes(typeof(JobAttribute), true);

            if (jobAttributes.Length <= 0)
                continue;

            if (Activator.CreateInstance(type) is not IJobHandler handler)
                continue;

            _scheduler?.ScheduleJob(handler.Job, handler.Trigger);

            Logger.Info(
                $"JobHandler [{type.FullName}] JobName: {handler.JobName} TriggerName: {handler.TriggerName} GroupName:{handler.GroupName} registration succeeded");
        }
    }

    public void Start()
    {
        RegisterHandler();
    }

    public async void Stop()
    {
        if (_scheduler != null)
            await _scheduler.Shutdown();
    }
}
