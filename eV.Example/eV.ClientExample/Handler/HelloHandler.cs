// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.ClientExample.Object.Receive;
using eV.ClientExample.Object.Send;
using eV.EasyLog;
using eV.Routing.Attributes;
using eV.Routing.Interface;
using eV.Unity;
namespace eV.ClientExample.Handler;

[ReceiveMessageHandler]
public class HelloHandler : HandlerBase<HelloSend>
{
    protected override void Handle(ISession session, HelloSend content)
    {
        if (content.Text == null)
            return;
        HellolReceive helloWorldClient = new()
        {
            Text = content.Text
        };
        session.Send(helloWorldClient);
        Logger.Info(helloWorldClient.Text);
    }
}
