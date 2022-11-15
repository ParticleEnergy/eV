// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using log4net;
using ILog = eV.Module.EasyLog.Interface.ILog;

namespace eV.Framework.Server.Logger;

public class ServerLog : ILog
{
    private readonly log4net.ILog _logger;

    public ServerLog(string loggerName)
    {
        _logger = LogManager.GetLogger(loggerName);
    }

    public void Debug(object message)
    {
        _logger.Debug(message);
    }

    public void Debug(object message, Exception exception)
    {
        _logger.Debug(message, exception);
    }

    public void Info(object message)
    {
        _logger.Info(message);
    }

    public void Info(object message, Exception exception)
    {
        _logger.Info(message, exception);
    }

    public void Warn(object message)
    {
        _logger.Warn(message);
    }

    public void Warn(object message, Exception exception)
    {
        _logger.Warn(message, exception);
    }

    public void Error(object message)
    {
        _logger.Error(message);
    }

    public void Error(object message, Exception exception)
    {
        _logger.Error(message, exception);
    }

    public void Fatal(object message)
    {
        _logger.Fatal(message);
    }

    public void Fatal(object message, Exception exception)
    {
        _logger.Fatal(message, exception);
    }
}
