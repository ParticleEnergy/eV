// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using Confluent.Kafka;
using eV.Module.EasyLog;
namespace eV.Module.Queue.Kafka.Handler;

public class LogHandler<TKey, TValue>
{
    public void ProducerErrorHandler(IProducer<TKey, TValue> _, LogMessage message) => Log(message);

    public void ConsumerErrorHandler(IConsumer<TKey, TValue> _, LogMessage message) => Log(message);

    private static void Log(LogMessage message)
    {
        switch (message.Level)
        {
            case SyslogLevel.Emergency:
                Logger.Fatal(message.Message);
                break;
            case SyslogLevel.Alert:
                Logger.Warn(message.Message);
                break;
            case SyslogLevel.Critical:
                Logger.Debug(message.Message);
                break;
            case SyslogLevel.Error:
                Logger.Error(message.Message);
                break;
            case SyslogLevel.Warning:
                Logger.Warn(message.Message);
                break;
            case SyslogLevel.Notice:
                Logger.Debug(message.Message);
                break;
            case SyslogLevel.Info:
                Logger.Info(message.Message);
                break;
            case SyslogLevel.Debug:
                Logger.Debug(message.Message);
                break;
            default:
                Logger.Info(message.Message);
                break;
        }
    }
}
