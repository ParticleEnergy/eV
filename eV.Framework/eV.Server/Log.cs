// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using log4net;
using ILog = eV.EasyLog.Interface.ILog;
namespace eV.Server
{
    public class Log : ILog
    {
        private log4net.ILog _logger = LogManager.GetLogger(DefaultSetting.LoggerName);
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
}
