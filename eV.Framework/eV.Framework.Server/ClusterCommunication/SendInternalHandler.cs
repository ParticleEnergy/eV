// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Framework.Server.Object;
using eV.Module.Cluster.Base;

namespace eV.Framework.Server.ClusterCommunication;

public class SendInternalHandler : InternalHandlerBase<SendBySessionIdPackage>
{
    public override bool IsMultipleSubscribers { get; set; } = false;

    protected override async Task Handle(SendBySessionIdPackage data)
    {
        if (data.SessionId != string.Empty && data.Data != null)
        {
            await ServerSession.Send(data.SessionId, data.Data);
        }
    }
}
