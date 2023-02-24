// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Text;
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
    public event Action<string>? CreateGroupAction;
    public event Action<string>? DeleteGroupAction;

    private const string GroupIdPrefix = "eV-Cluster-GroupId";
    private const string TopicPrefix = "eV-Cluster-Topic";

    private static readonly string s_sendTopicFormat = "{0}-Cluster:{1}-Node:{2}-Send";

    private readonly string _sendTopic;
    private readonly string _sendGroupTopic;
    private readonly string _sendBroadcastTopic;
    private readonly string _createGroupTopic;
    private readonly string _deleteGroupTopic;

    private readonly string _sendConsumeGroupId;
    private readonly string _sendGroupConsumeGroupId;
    private readonly string _sendBroadcastConsumeGroupId;
    private readonly string _createGroupConsumeGroupId;
    private readonly string _deleteGroupConsumeGroupId;

    private readonly int _consumeSendPipelineNumber;
    private readonly int _consumeSendGroupPipelineNumber;
    private readonly int _consumeSendBroadcastPipelineNumber;
    private readonly int _consumeCreateGroupPipelineNumber;
    private readonly int _consumeDeleteGroupPipelineNumber;

    private readonly ISessionRegistrationAuthority _sessionRegistrationAuthority;

    private readonly Kafka _kafka;

    public CommunicationQueue(
        string clusterName,
        string nodeName,
        int consumeSendPipelineNumber,
        int consumeSendGroupPipelineNumber,
        int consumeSendBroadcastPipelineNumber,
        int consumeCreateGroupPipelineNumber,
        int consumeDeleteGroupPipelineNumber,
        ISessionRegistrationAuthority sessionRegistrationAuthority,
        KeyValuePair<ProducerConfig, ConsumerConfig> kafkaOption
    )
    {
        _clusterName = clusterName;
        _nodeName = nodeName;

        _consumeSendPipelineNumber = consumeSendPipelineNumber;
        _consumeSendGroupPipelineNumber = consumeSendGroupPipelineNumber;
        _consumeSendBroadcastPipelineNumber = consumeSendBroadcastPipelineNumber;
        _consumeCreateGroupPipelineNumber = consumeCreateGroupPipelineNumber;
        _consumeDeleteGroupPipelineNumber = consumeDeleteGroupPipelineNumber;

        _sessionRegistrationAuthority = sessionRegistrationAuthority;

        _sendTopic = string.Format(s_sendTopicFormat, TopicPrefix, _clusterName, _nodeName);
        _sendGroupTopic = $"{TopicPrefix}-Cluster:{_clusterName}-SendGroup";
        _sendBroadcastTopic = $"{TopicPrefix}-Cluster:{_clusterName}-SendBroadcast";
        _createGroupTopic = $"{TopicPrefix}-Cluster:{_clusterName}-CreateGroup";
        _deleteGroupTopic = $"{TopicPrefix}-Cluster:{_clusterName}-DeleteGroup";

        _sendConsumeGroupId = $"{GroupIdPrefix}-Cluster:{_clusterName}-Node:{_nodeName}-Send";
        _sendGroupConsumeGroupId = $"{GroupIdPrefix}-Cluster:{_clusterName}-SendGroup";
        _sendBroadcastConsumeGroupId = $"{GroupIdPrefix}-Cluster:{_clusterName}-SendBroadcast";
        _createGroupConsumeGroupId = $"{GroupIdPrefix}-Cluster:{_clusterName}-CreateGroup";
        _deleteGroupConsumeGroupId = $"{GroupIdPrefix}-Cluster:{_clusterName}-DeleteGroup";
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
        if (SendGroupAction == null)
            return;
        _kafka.Produce(_sendGroupTopic, $"{_nodeName}:{groupId}", data);
    }

    public void SendBroadcast(byte[] data)
    {
        if (SendBroadcastAction == null)
            return;
        _kafka.Produce(_sendBroadcastTopic, _nodeName, data);
    }

    public void CreateGroup(string groupId)
    {
        if (CreateGroupAction == null)
            return;
        _kafka.Produce(_createGroupTopic, _nodeName, Encoding.UTF8.GetBytes(groupId));
    }

    public void DeleteGroup(string groupId)
    {
        if (DeleteGroupAction == null)
            return;
        _kafka.Produce(_deleteGroupTopic, _nodeName, Encoding.UTF8.GetBytes(groupId));
    }

    public void Start()
    {
        for (int i = 0; i < _consumeSendPipelineNumber; ++i)
        {
            new Task(ConsumeSend).Start();
        }

        for (int i = 0; i < _consumeSendGroupPipelineNumber; ++i)
        {
            new Task(ConsumeSendGroup).Start();
        }

        for (int i = 0; i < _consumeSendBroadcastPipelineNumber; ++i)
        {
            new Task(ConsumeSendBroadcast).Start();
        }

        for (int i = 0; i < _consumeCreateGroupPipelineNumber; ++i)
        {
            new Task(ConsumeCreateGroup).Start();
        }

        for (int i = 0; i < _consumeDeleteGroupPipelineNumber; ++i)
        {
            new Task(ConsumeDeleteGroup).Start();
        }
    }

    public void Stop()
    {
        _kafka.Stop();
    }

    private void ConsumeSend()
    {
        if (SendAction != null)
        {
            _kafka.Consume(
                _sendConsumeGroupId,
                _sendTopic,
                delegate(ConsumeResult<string, byte[]> result)
                {
                    SendAction.Invoke(result.Message.Key, result.Message.Value);
                });
        }
        else
        {
            Logger.Error("SendAction not defined");
        }
    }

    private void ConsumeSendGroup()
    {
        if (SendGroupAction != null)
        {
            _kafka.Consume(
                _sendGroupConsumeGroupId,
                _sendGroupTopic,
                delegate(ConsumeResult<string, byte[]> result)
                {
                    string[] queueData = result.Message.Key.Split(":");
                    if (queueData[0] == _nodeName)
                        return;
                    SendGroupAction.Invoke(queueData[1], result.Message.Value);
                });
        }
        else
        {
            Logger.Error("SendGroupAction not defined");
        }
    }

    private void ConsumeSendBroadcast()
    {
        if (SendBroadcastAction != null)
        {
            _kafka.Consume(
                _sendBroadcastConsumeGroupId,
                _sendBroadcastTopic,
                delegate(ConsumeResult<string, byte[]> result)
                {
                    if (result.Message.Key == _nodeName)
                        return;

                    SendBroadcastAction.Invoke(result.Message.Value);
                });
        }
        else
        {
            Logger.Error("SendBroadcastAction not defined");
        }
    }

    private void ConsumeCreateGroup()
    {
        if (CreateGroupAction != null)
        {
            _kafka.Consume(
                _createGroupConsumeGroupId,
                _createGroupTopic,
                delegate(ConsumeResult<string, byte[]> result)
                {
                    if (result.Message.Key == _nodeName)
                        return;

                    CreateGroupAction.Invoke(Encoding.UTF8.GetString(result.Message.Value));
                });
        }
        else
        {
            Logger.Error("CreateGroupAction not defined");
        }
    }

    private void ConsumeDeleteGroup()
    {
        if (DeleteGroupAction != null)
        {
            _kafka.Consume(
                _deleteGroupConsumeGroupId,
                _deleteGroupTopic,
                delegate(ConsumeResult<string, byte[]> result)
                {
                    if (result.Message.Key == _nodeName)
                        return;

                    DeleteGroupAction.Invoke(Encoding.UTF8.GetString(result.Message.Value));
                });
        }
        else
        {
            Logger.Error("DeleteGroupAction not defined");
        }
    }
}
