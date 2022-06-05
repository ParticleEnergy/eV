// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Cluster.Interface;
namespace eV.Module.Cluster;

public class Cluster
{
    private readonly ICommunicationQueue _communicationQueue;
    private readonly ISessionRegistrationAuthority _sessionRegistrationAuthority;
    private readonly ClusterSession _clusterSession;

    public Cluster(ClusterSetting setting)
    {
        string nodeName = Guid.NewGuid().ToString();
        _sessionRegistrationAuthority = new SessionRegistrationAuthority(setting.ClusterName, nodeName, setting.RedisOption);

        _communicationQueue = new CommunicationQueue(
            setting.ClusterName,
            nodeName,
            setting.ConsumeSendPipelineNumber,
            setting.ConsumeSendGroupPipelineNumber,
            setting.ConsumeSendBroadcastPipelineNumber,
            _sessionRegistrationAuthority,
            setting.KafkaOption
        );
        _communicationQueue.SendAction += setting.SendAction;
        _communicationQueue.SendGroupAction += setting.SendGroupAction;
        _communicationQueue.SendBroadcastAction += setting.SendBroadcastAction;

        _clusterSession = new ClusterSession(_sessionRegistrationAuthority, _communicationQueue);
    }

    public ClusterSession GetClusterSession()
    {
        return _clusterSession;
    }

    public void Start()
    {
        _sessionRegistrationAuthority.Start();
        _communicationQueue.Start();
    }

    public void Stop()
    {
        _communicationQueue.Stop();
        _sessionRegistrationAuthority.Stop();
    }
}
