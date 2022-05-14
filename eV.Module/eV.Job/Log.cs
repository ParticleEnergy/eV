using Quartz.Logging;
using LogLevel = Quartz.Logging.LogLevel;
using EasyLogger = eV.EasyLog.Logger;
namespace eV.Job;

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
                    EasyLogger.Fatal(message, exception);
                    break;
            }
            return true;
        };
    }

    public IDisposable OpenNestedContext(string message)
    {
        throw new NotImplementedException();
    }

    public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
    {
        throw new NotImplementedException();
    }
}
