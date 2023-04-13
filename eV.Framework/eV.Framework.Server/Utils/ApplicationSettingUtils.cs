// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using System.Globalization;
using eV.Module.GameProfile;
using eV.Module.Routing.Interface;

namespace eV.Framework.Server.Utils;

public static class ApplicationSettingUtils
{
    public static void SetProfile<T>(T data)
    {
        Profile.OnLoad += delegate { Profile.AssignmentConfigObject(data); };
    }

    public static void SetServerOnConnected(SessionEvent onConnected)
    {
        ServerEvent.ServerOnConnected = onConnected;
    }

    public static void SetServerSessionOnActivate(SessionEvent sessionOnActivate)
    {
        ServerEvent.ServerSessionOnActivate = sessionOnActivate;
    }

    public static void SetServerSessionOnRelease(SessionEvent sessionOnRelease)
    {
        ServerEvent.ServerSessionOnRelease = sessionOnRelease;
    }

    public static void SetServerOnStart(Action onStart)
    {
        ServerEvent.ServerOnStart = onStart;
    }

    public static void SetServerOnStop(Action onStop)
    {
        ServerEvent.ServerOnStop = onStop;
    }

    public static void SetCultureInfo(string name)
    {
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(name, true) { DateTimeFormat = { ShortDatePattern = "yyyy-MM-dd", FullDateTimePattern = "yyyy-MM-dd HH:mm:ss", LongTimePattern = "HH:mm:ss" } };
    }
}
