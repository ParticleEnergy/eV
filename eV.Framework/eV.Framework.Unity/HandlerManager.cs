// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Routing;
namespace eV.Framework.Unity;

public static class HandlerManager
{
    public static T? GetHandler<T>()
    {
        var handler = Dispatch.GetHandler<T>();
        return handler == null ? default : (T)handler;
    }
}
