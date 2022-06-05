// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using EasyLogger = eV.Module.EasyLog.Logger;
namespace eV.ServerExample.Core;

public class Logger : ILogger
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        string message = $"{formatter(state, exception)}";
        switch (logLevel)
        {
            case LogLevel.Trace:
                if (exception == null)
                    EasyLogger.Debug(message);
                else
                    EasyLogger.Debug(message, exception);
                break;
            case LogLevel.Debug:
                if (exception == null)
                    EasyLogger.Debug(message);
                else
                    EasyLogger.Debug(message, exception);
                break;
            case LogLevel.Information:
                if (exception == null)
                    EasyLogger.Info(message);
                else
                    EasyLogger.Info(message, exception);
                break;
            case LogLevel.Warning:
                if (exception == null)
                    EasyLogger.Warn(message);
                else
                    EasyLogger.Warn(message, exception);
                break;
            case LogLevel.Error:
                if (exception == null)
                    EasyLogger.Error(message);
                else
                    EasyLogger.Error(message, exception);
                break;
            case LogLevel.Critical:
                if (exception == null)
                    EasyLogger.Debug(message);
                else
                    EasyLogger.Debug(message, exception);
                break;
            case LogLevel.None:
                if (exception == null)
                    EasyLogger.Info(message);
                else
                    EasyLogger.Info(message, exception);
                break;
            default:
                if (exception == null)
                    EasyLogger.Info(message);
                else
                    EasyLogger.Info(message, exception);
                break;
        }
    }
    public bool IsEnabled(LogLevel logLevel)
    {
        throw new NotImplementedException();
    }
    public IDisposable BeginScope<TState>(TState state)
    {
        throw new NotImplementedException();
    }
}
