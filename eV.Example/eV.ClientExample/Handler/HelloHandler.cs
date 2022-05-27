// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Framework.Client;
using eV.Module.EasyLog;
using eV.Module.Routing.Attributes;
using eV.Module.Routing.Interface;
using eV.PublicObject.ClientObject;
using eV.PublicObject.ServerObject;
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
