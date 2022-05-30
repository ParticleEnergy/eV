// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Cluster.Interface;
namespace eV.Module.Cluster;

public class CommunicationQueue : ICommunicationQueue
{
    public event Func<string, byte[], bool>? SendAction;
    private event Action<string, byte[]>? SendGroupAction;
    private event Action<byte[]>? SendBroadcastAction;
    private readonly int _workerNumber;
    public readonly string QueueSend;
    public readonly string QueueSendGroup;
    public readonly string QueueSendBroadcast;


    public CommunicationQueue( string queueName, int workerNumber)
    {
        QueueSend = $"{queueName}-send";
        QueueSendGroup = $"{queueName}-send-group";
        QueueSendBroadcast = $"{queueName}-send-broadcast";

        _workerNumber = workerNumber;
    }

    public void SendProducer(string sessionId, byte[] data)
    {

    }

    public void SendGroupProducer(string groupId, byte[] data)
    {

    }

    public void SendBroadcastProducer(byte[] data)
    {
    }

    private void Consumer()
    {
    }
}
