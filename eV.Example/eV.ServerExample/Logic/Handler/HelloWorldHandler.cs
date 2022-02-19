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
public class HelloWorldHandler : HandlerBase<HellolReceive>
{
    public HelloWorldHandler()
    {
        Skip = true;
    }

    protected override void Handle(ISession session, HellolReceive content)
    {
        if (content.Text == null)
            return;
        HelloSend helloWorldClient = new()
        {
            Text = content.Text
        };
        session.Send(helloWorldClient);
        Logger.Info(helloWorldClient.Text + DateTime.Now);
    }
}
