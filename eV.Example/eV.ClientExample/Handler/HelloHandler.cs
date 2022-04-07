// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.


using eV.DataStruct.ClientStruct;
using eV.DataStruct.ServerStruct;
using eV.EasyLog;
using eV.Routing.Attributes;
using eV.Routing.Interface;
using eV.Unity;
namespace eV.ClientExample.Handler;

[ReceiveMessageHandler]
public class HelloHandler : HandlerBase<HelloServer>
{
    protected override void Handle(ISession session, HelloServer content)
    {
        if (content.Text == null)
            return;
        HelloClient helloWorldClient = new()
        {
            Text = content.Text
        };
        session.Send(helloWorldClient);
        Logger.Info(helloWorldClient.Text);
    }
}
