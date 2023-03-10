// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.EasyLog.Interface;

namespace eV.Module.EasyLog;

public class Log : ILog
{
    public bool IsDebug()
    {
        return true;
    }

    public void Debug(object message)
    {
        Console.WriteLine(message);
    }

    public void Debug(object message, Exception exception)
    {
        Console.WriteLine(message);
        Console.WriteLine(exception);
    }

    public void Info(object message)
    {
        Console.WriteLine(message);
    }

    public void Info(object message, Exception exception)
    {
        Console.WriteLine(message);
        Console.WriteLine(exception);
    }

    public void Warn(object message)
    {
        Console.WriteLine(message);
    }

    public void Warn(object message, Exception exception)
    {
        Console.WriteLine(message);
        Console.WriteLine(exception);
    }

    public void Error(object message)
    {
        Console.WriteLine(message);
    }

    public void Error(object message, Exception exception)
    {
        Console.WriteLine(message);
        Console.WriteLine(exception);
    }

    public void Fatal(object message)
    {
        Console.WriteLine(message);
    }

    public void Fatal(object message, Exception exception)
    {
        Console.WriteLine(message);
        Console.WriteLine(exception);
    }
}
