// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.


using eV.Client;
using eV.PublicObject.ClientObject;
using eV.PublicObject.ServerObject;
using eV.EasyLog;
using eV.Routing.Attributes;
using eV.Routing.Interface;
namespace eV.ClientExample.Handler;

[ReceiveMessageHandler]
public class HelloHandler : HandlerBase<HelloServerMessage>
{
    protected override void Handle(ISession session, HelloServerMessage content)
    {
        if (content.Text == null)
            return;
        HelloClientMessage helloWorldClient = new()
        {
            Text = content.Text
        };
        // session.Send(helloWorldClient);
        Logger.Info(helloWorldClient.Text);
    }
}
