// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Cluster.Interface;
namespace eV.Module.Cluster;

public class Cluster
{
    public event Func<string, byte[], bool>? SendAction;
    public event Action<string, byte[]>? SendGroupAction;
    public event Action<byte[]>? SendBroadcastAction;

    private readonly string _nodeName = "todo";
    private readonly ICommunicationQueue _communicationQueue;
    private readonly ISessionRegistrationAuthority _sessionRegistrationAuthority;
    private readonly ClusterSession _clusterSession;

    public Cluster(ClusterSetting setting)
    {
        _sessionRegistrationAuthority = new SessionRegistrationAuthority(setting.ClusterName, _nodeName, setting.RedisOption);

        _communicationQueue = new CommunicationQueue(
            setting.ClusterName,
            _nodeName,
            setting.ConsumeSendPipelineNumber,
            setting.ConsumeSendGroupPipelineNumber,
            setting.ConsumeSendBroadcastPipelineNumber,
            _sessionRegistrationAuthority,
            setting.KafkaOption
        );

        _clusterSession = new ClusterSession(_sessionRegistrationAuthority, _communicationQueue);
    }

    public ClusterSession GetClusterSession()
    {
        return _clusterSession;
    }

    public void Start()
    {
        _sessionRegistrationAuthority.Start();
        _communicationQueue.SendAction += SendAction;
        _communicationQueue.SendGroupAction += SendGroupAction;
        _communicationQueue.SendBroadcastAction += SendBroadcastAction;
        _communicationQueue.Start();
    }

    public void Stop()
    {
        _communicationQueue.Stop();
        _sessionRegistrationAuthority.Stop();
    }
}
