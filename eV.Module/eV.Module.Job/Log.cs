// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using Quartz.Logging;
using EasyLogger = eV.Module.EasyLog.Logger;

namespace eV.Module.Job;

public class Log : ILogProvider
{
    public Logger GetLogger(string name)
    {
        return (level, func, exception, parameters) =>
        {
            string message = $"func: {func()} parameters: {parameters}";
            switch (level)
            {
                case LogLevel.Debug:
                    EasyLogger.Debug(message, exception);
                    break;
                case LogLevel.Info:
                    EasyLogger.Info(message, exception);
                    break;
                case LogLevel.Warn:
                    EasyLogger.Warn(message, exception);
                    break;
                case LogLevel.Error:
                    EasyLogger.Error(message, exception);
                    break;
                case LogLevel.Fatal:
                    EasyLogger.Fatal(message, exception);
                    break;
                case LogLevel.Trace:
                default:
                    EasyLogger.Debug(message, exception);
                    break;
            }

            return true;
        };
    }

    public IDisposable OpenNestedContext(string message)
    {
        return NullScope.Instance;
    }

    public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
    {
        return NullScope.Instance;
    }

    public sealed class NullScope : IDisposable
    {
        private NullScope()
        {
        }

        public static NullScope Instance { get; } = new();

        public void Dispose()
        {
        }
    }
}
