// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Framework.Server.Utils;
using eV.Module.Routing.Interface;
using Microsoft.AspNetCore.Builder;

namespace eV.Framework.Server;

public static class WebApplicationExtensions
{
    public static WebApplication SetProfile<T>(this WebApplication webApplication, T data)
    {
        ApplicationSettingUtils.SetProfile(data);
        return webApplication;
    }

    public static WebApplication SetServerOnConnected(this WebApplication webApplication, SessionEvent onConnected)
    {
        ApplicationSettingUtils.SetServerOnConnected(onConnected);
        return webApplication;
    }

    public static WebApplication SetServerSessionOnActivate(this WebApplication webApplication, SessionEvent sessionOnActivate)
    {
        ApplicationSettingUtils.SetServerSessionOnActivate(sessionOnActivate);
        return webApplication;
    }

    public static WebApplication SetServerSessionOnRelease(this WebApplication webApplication, SessionEvent sessionOnRelease)
    {
        ApplicationSettingUtils.SetServerSessionOnRelease(sessionOnRelease);
        return webApplication;
    }

    public static WebApplication SetServerOnStart(this WebApplication webApplication, Action onStart)
    {
        ApplicationSettingUtils.SetServerOnStart(onStart);
        return webApplication;
    }

    public static WebApplication SetServerOnStop(this WebApplication webApplication, Action onStop)
    {
        ApplicationSettingUtils.SetServerOnStop(onStop);
        return webApplication;
    }

    public static WebApplication SetCultureInfo(this WebApplication webApplication, string name)
    {
        ApplicationSettingUtils.SetCultureInfo(name);
        return webApplication;
    }
}
