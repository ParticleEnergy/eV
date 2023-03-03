// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Framework.Server.Object;
using eV.Module.Cluster.Base;

namespace eV.Framework.Server.ClusterCommunication;

public class SendBroadcastInternalHandler : InternalHandlerBase<InternalSendBroadcastPackage>
{
    public override bool IsMultipleSubscribers { get; set; } = true;

    protected override async Task Handle(InternalSendBroadcastPackage data)
    {
        if (data.SelfSessionId != string.Empty && data.Data != null)
        {
            await ServerSession.SendBroadcast(data.SelfSessionId, data.Data);
        }
    }
}
