// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using eV.Routing.Interface;
namespace eV.Routing
{
    public class Route : IRoute
    {
        public Route(IHandler handler, Type contentType)
        {
            Handler = handler;
            ContentType = contentType;
        }
        public IHandler Handler
        {
            get;
        }
        public Type ContentType
        {
            get;
        }
    }
}
