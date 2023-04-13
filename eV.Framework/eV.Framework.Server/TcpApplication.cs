// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Framework.Server.Utils;
using eV.Module.Routing.Interface;
using Microsoft.Extensions.Hosting;

namespace eV.Framework.Server;

public sealed class TcpApplication
{
    private readonly IHost _host;

    public TcpApplication(IHost host)
    {
        _host = host;
    }

    public void Run()
    {
        _host.Run();
    }

    public async Task RunAsync()
    {
        await _host.RunAsync();
    }

    public TcpApplication SetProfile<T>(T data)
    {
        ApplicationSettingUtils.SetProfile(data);
        return this;
    }

    public TcpApplication SetServerOnConnected(SessionEvent onConnected)
    {
        ApplicationSettingUtils.SetServerOnConnected(onConnected);
        return this;
    }

    public TcpApplication SetServerSessionOnActivate(SessionEvent sessionOnActivate)
    {
        ApplicationSettingUtils.SetServerSessionOnActivate(sessionOnActivate);
        return this;
    }

    public TcpApplication SetServerSessionOnRelease(SessionEvent sessionOnRelease)
    {
        ApplicationSettingUtils.SetServerSessionOnRelease(sessionOnRelease);
        return this;
    }

    public TcpApplication SetServerOnStart(Action onStart)
    {
        ApplicationSettingUtils.SetServerOnStart(onStart);
        return this;
    }

    public TcpApplication SetServerOnStop(Action onStop)
    {
        ApplicationSettingUtils.SetServerOnStop(onStop);
        return this;
    }

    public TcpApplication SetCultureInfo(string name)
    {
        ApplicationSettingUtils.SetCultureInfo(name);
        return this;
    }
}
