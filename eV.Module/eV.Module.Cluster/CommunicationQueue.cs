// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using Confluent.Kafka;
using eV.Module.Cluster.Interface;
using eV.Module.EasyLog;
namespace eV.Module.Cluster;

public class CommunicationQueue : ICommunicationQueue
{
    private readonly string _clusterName;
    private readonly string _nodeName;

    public event Func<string, byte[], bool>? SendAction;
    public event Action<string, byte[]>? SendGroupAction;
    public event Action<byte[]>? SendBroadcastAction;

    private const string GroupIdPrefix = "eV-Cluster-GroupId";
    private const string TopicPrefix = "eV-Cluster-Topic";

    private static readonly string s_sendTopicFormat = $"{0}-Cluster:{1}-Node:{2}-Send";

    private readonly string _sendTopic;
    private readonly string _sendGroupTopic;
    private readonly string _sendBroadcastTopic;

    private readonly int _consumeSendPipelineNumber;
    private readonly int _consumeSendGroupPipelineNumber;
    private readonly int _consumeSendBroadcastPipelineNumber;

    private readonly ISessionRegistrationAuthority _sessionRegistrationAuthority;

    private readonly Kafka _kafka;

    public CommunicationQueue(string clusterName, string nodeName, int consumeSendPipelineNumber, int consumeSendGroupPipelineNumber, int consumeSendBroadcastPipelineNumber, ISessionRegistrationAuthority sessionRegistrationAuthority, KeyValuePair<ProducerConfig, ConsumerConfig> kafkaOption)
    {
        _clusterName = clusterName;
        _nodeName = nodeName;

        _consumeSendPipelineNumber = consumeSendPipelineNumber;
        _consumeSendGroupPipelineNumber = consumeSendGroupPipelineNumber;
        _consumeSendBroadcastPipelineNumber = consumeSendBroadcastPipelineNumber;

        _sessionRegistrationAuthority = sessionRegistrationAuthority;

        _sendTopic = string.Format(s_sendTopicFormat, TopicPrefix, _clusterName, _nodeName);
        _sendGroupTopic = $"{GroupIdPrefix}-Cluster:{_clusterName}-SendGroup";
        _sendBroadcastTopic = $"{GroupIdPrefix}-Cluster:{_clusterName}-SendBroadcast";

        _kafka = new Kafka(_clusterName, _nodeName, kafkaOption);
    }

    public void Send(string sessionId, byte[] data)
    {
        if (SendAction == null)
            return;

        string nodeName = _sessionRegistrationAuthority.GetNodeName(sessionId);
        if (!nodeName.Equals(""))
        {
            _kafka.Produce(string.Format(s_sendTopicFormat, TopicPrefix, _clusterName, nodeName), sessionId, data);
        }
        else
        {
            Logger.Warn("The session: {sessionId} was not found in the registry");
        }
    }

    public void SendGroup(string groupId, byte[] data)
    {
        _kafka.Produce(_sendGroupTopic, $"{_nodeName}:{groupId}", data);
    }

    public void SendBroadcast(byte[] data)
    {
        _kafka.Produce(_sendBroadcastTopic, _nodeName, data);
    }

    public void Start()
    {
        for (int i = 0; i < _consumeSendPipelineNumber;)
        {
            new Task(ConsumeSend).Start();
        }

        for (int i = 0; i < _consumeSendGroupPipelineNumber;)
        {
            new Task(ConsumeSendGroup).Start();
        }

        for (int i = 0; i < _consumeSendBroadcastPipelineNumber;)
        {
            new Task(ConsumeSendBroadcast).Start();
        }
    }

    public void Stop()
    {
        _kafka.Stop();
    }

    private void ConsumeSend()
    {
        _kafka.Consume($"{GroupIdPrefix}-Cluster:{_clusterName}-Node:{_nodeName}-Send", _sendTopic, delegate(ConsumeResult<string, byte[]> result)
        {
            if (SendAction != null)
            {
                SendAction.Invoke(result.Message.Key, result.Message.Value);
            }
            else
            {
                Logger.Error("SendAction not defined");
            }
        });
    }

    private void ConsumeSendGroup()
    {
        _kafka.Consume($"{GroupIdPrefix}-Cluster:{_clusterName}-SendGroup", _sendGroupTopic, delegate(ConsumeResult<string, byte[]> result)
        {
            string[] queueData = result.Message.Key.Split(":");
            if (queueData[0] == _nodeName)
                return;
            if (SendGroupAction != null)
            {
                SendGroupAction.Invoke(queueData[1], result.Message.Value);
            }
            else
            {
                Logger.Error("SendGroupAction not defined");
            }
        });
    }

    private void ConsumeSendBroadcast()
    {
        _kafka.Consume($"{GroupIdPrefix}-Cluster:{_clusterName}-SendBroadcast", _sendBroadcastTopic, delegate(ConsumeResult<string, byte[]> result)
        {
            string[] queueData = result.Message.Key.Split(":");
            if (queueData[0] == _nodeName)
                return;

            if (SendBroadcastAction != null)
            {
                SendBroadcastAction.Invoke(result.Message.Value);
            }
            else
            {
                Logger.Error("SendBroadcastAction not defined");
            }
        });
    }
}
