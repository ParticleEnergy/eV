// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.DataStruct.ClientStruct;
using eV.DataStruct.ServerStruct;
using eV.EasyLog;
using eV.Routing.Attributes;
using eV.Routing.Interface;
using eV.Server.Base;
namespace eV.ServerExample.Logic.Handler;

[ReceiveMessageHandler]
public class HelloWorldHandler : HandlerBase<HelloClient>
{
    public HelloWorldHandler()
    {
        Skip = true;
    }

    protected override void Handle(ISession session, HelloClient content)
    {
        if (content.Text == null)
            return;
        HelloServer helloWorldClient = new()
        {
            Text = content.Text
        };
        session.Send(helloWorldClient);
        Logger.Info(helloWorldClient.Text + DateTime.Now);
    }
}
