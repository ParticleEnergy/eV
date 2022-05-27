// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Routing.Interface;
namespace eV.Module.Routing;

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
