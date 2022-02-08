// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.


using System;
using eV.Routing;
using eV.Routing.Attributes;
using eV.Routing.Interface;
using eV.Server.Base;
namespace eV.Test.Handlers
{
    [ReceiveMessageHandler]
    public class Index : HandlerBase<IndexParam>
    {

        protected override void Handle(ISession session, IndexParam param)
        {
            Console.WriteLine(param.Name);
        }

    }

    public class IndexParam
    {
        public string Name = "index";
    }

    [SendMessageAttribute]
    public class SendMessageTest
    {

    }
}
