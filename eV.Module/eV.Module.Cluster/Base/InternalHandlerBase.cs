// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using System.Text.Json;
using eV.Module.Cluster.Interface;
using eV.Module.EasyLog;
using StackExchange.Redis;

namespace eV.Module.Cluster.Base;

public abstract class InternalHandlerBase<T> : IInternalHandler
{
    protected abstract Task Consume(T data);

    public async Task Run(CancellationToken cancellationToken)
    {
        if (CommunicationManager.Instance == null)
            return;

        ConsumerIdentifier? consumerIdentifier = CommunicationManager.Instance.GetConsumerIdentifier(typeof(T));
        if (consumerIdentifier == null)
        {
            Logger.Error($"InternalMessage {typeof(T).Name} not found");
            return;
        }

        string stream = consumerIdentifier.GetStream(CommunicationManager.Instance.NodeId);
        string group = consumerIdentifier.GetGroup(CommunicationManager.Instance.NodeId);
        string consumer = consumerIdentifier.GetConsumer(CommunicationManager.Instance.NodeId);

        while (!cancellationToken.IsCancellationRequested)
        {
            var messages = CommunicationManager.Instance.Consume(stream, group, consumer);

            if (messages.Length <= 0)
                continue;

            List<RedisValue> deleteIds = new();
            foreach (StreamEntry message in messages)
            {
                try
                {
                    var data = JsonSerializer.Deserialize<T>(message.Values.First().Value.ToString());
                    if (data == null)
                    {
                        continue;
                    }

                    await Consume(data);
                    deleteIds.Add(message.Id);
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message, e);
                }
            }

            if (deleteIds.Count > 0)
                await CommunicationManager.Instance.DeleteMessage(stream, deleteIds.ToArray());
        }
    }
}
