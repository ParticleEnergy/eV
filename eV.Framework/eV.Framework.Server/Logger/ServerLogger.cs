// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using log4net;
using Microsoft.Extensions.Logging;
using _logger = eV.Module.EasyLog.Logger;

namespace eV.Framework.Server.Logger;

public class ServerLogger : ILogger
{
    private readonly ILog _logger;

    public ServerLogger(string categoryName)
    {
        _logger = LogManager.GetLogger(categoryName);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        string message = $"{formatter(state, exception)}";
        switch (logLevel)
        {
            case LogLevel.Trace:
                if (exception == null)
                    _logger.Debug(message);
                else
                    _logger.Debug(message, exception);
                break;
            case LogLevel.Debug:
                if (exception == null)
                    _logger.Debug(message);
                else
                    _logger.Debug(message, exception);
                break;
            case LogLevel.Information:
                if (exception == null)
                    _logger.Info(message);
                else
                    _logger.Info(message, exception);
                break;
            case LogLevel.Warning:
                if (exception == null)
                    _logger.Warn(message);
                else
                    _logger.Warn(message, exception);
                break;
            case LogLevel.Error:
                if (exception == null)
                    _logger.Error(message);
                else
                    _logger.Error(message, exception);
                break;
            case LogLevel.Critical:
                if (exception == null)
                    _logger.Debug(message);
                else
                    _logger.Debug(message, exception);
                break;
            case LogLevel.None:
                if (exception == null)
                    _logger.Info(message);
                else
                    _logger.Info(message, exception);
                break;
            default:
                if (exception == null)
                    _logger.Info(message);
                else
                    _logger.Info(message, exception);
                break;
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return NullScope.Instance;
    }
}
