// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Module.Cluster;

public static class Define
{
    public const string TopicQueueSend = "{0}-{1}-send";
    public const string TopicQueueSendGroup = "{0}-send-group";
    public const string TopicQueueSendBroadcast = "{0}-send-broadcast";

    public const string GroupIdDefault = "group-{0}-send";
    public const string GroupIdGroup = "group-{0}-send-group";
    public const string GroupIdBroadcast = "group-{0}-send-broadcast";
}
