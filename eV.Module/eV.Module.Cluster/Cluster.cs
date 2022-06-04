// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Cluster.Interface;
namespace eV.Module.Cluster;

public class Cluster
{
    private readonly string _nodeName = "todo";
    private readonly CommunicationQueue _communicationQueue;
    private readonly ISessionRegistrationAuthority _sessionRegistrationAuthority;
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
    }


}
