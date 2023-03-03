// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using System.Text.Json;
using eV.Module.Cluster.Interface;
using eV.Module.EasyLog;
using StackExchange.Redis;

namespace eV.Module.Cluster.Base;

public abstract class InternalHandlerBase<T> : IInternalHandler
{
    public virtual bool IsMultipleSubscribers { get; set; } = false;
    protected abstract Task Handle(T data);

    public void Run()
    {
        if (CommunicationManager.Instance == null)
            return;

        CommunicationManager.Instance.Subscribe(typeof(T), Action);
    }

    private async void Action(RedisChannel _, RedisValue value)
    {
        try
        {
            if (value.IsNull) return;

            var data = JsonSerializer.Deserialize<T>(value.ToString());
            if (data != null)
            {
                await Handle(data);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }
}
