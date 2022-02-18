// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.EasyLog;
using eV.Routing.Attributes;
using eV.Routing.Interface;
using eV.Server.Base;
using eV.ServerExample.Object.Receive;
using eV.ServerExample.Object.Send;
namespace eV.ServerExample.Logic.Handler;

[ReceiveMessageHandler]
public class HelloWorldHandler : HandlerBase<HellolWorldReceive>
{
    public HelloWorldHandler()
    {
        Skip = true;
    }

    protected override void Handle(ISession session, HellolWorldReceive content)
    {
        if (content.Text == null)
            return;
        HelloWorldSend helloWorldClient = new()
        {
            Text = content.Text
        };
        session.Send(helloWorldClient);
        Logger.Info(helloWorldClient.Text);
    }
}
