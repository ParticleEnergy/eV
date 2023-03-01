// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Module.EasyLog.Interface;

namespace eV.Module.EasyLog;

public static class Logger
{
    private static ILog s_log = new Log();

    public static void SetLogger(ILog log)
    {
        s_log = log;
    }

    public static bool IsDebug()
    {
        return s_log.IsDebug();
    }

    public static void Debug(object message)
    {
        s_log.Debug(message);
    }

    public static void Debug(object message, Exception exception)
    {
        s_log.Debug(message, exception);
    }

    public static void Info(object message)
    {
        s_log.Info(message);
    }

    public static void Info(object message, Exception exception)
    {
        s_log.Info(message, exception);
    }

    public static void Warn(object message)
    {
        s_log.Warn(message);
    }

    public static void Warn(object message, Exception exception)
    {
        s_log.Warn(message, exception);
    }

    public static void Error(object message)
    {
        s_log.Error(message);
    }

    public static void Error(object message, Exception exception)
    {
        s_log.Error(message, exception);
    }

    public static void Fatal(object message)
    {
        s_log.Fatal(message);
    }

    public static void Fatal(object message, Exception exception)
    {
        s_log.Fatal(message, exception);
    }
}
