// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Reflection;
using eV.Module.EasyLog;
using eV.Module.Queue.Attributes;
using eV.Module.Queue.Interface;

namespace eV.Module.Queue;

public class Queue
{
    private readonly string _assemblyString;

    public Queue(string assemblyString)
    {
        _assemblyString = assemblyString;
    }

    private void RegisterHandler()
    {
        Type[] allTypes = Assembly.Load(_assemblyString).GetExportedTypes();

        foreach (Type type in allTypes)
        {
            object[] jobAttributes = type.GetCustomAttributes(typeof(QueueAttribute), true);

            if (jobAttributes.Length <= 0)
                continue;

            if (Activator.CreateInstance(type) is not IQueueHandler handler)
                continue;

            handler.RunConsume();

            Logger.Info($"QueueHandler [{type.FullName}] QueueName: {handler.QueueName} registration succeeded");
        }
    }

    public void Start()
    {
        RegisterHandler();
    }
}
