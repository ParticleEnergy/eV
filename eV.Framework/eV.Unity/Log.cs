// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.EasyLog.Interface;
namespace eV.Unity;

public class Log : ILog
{
    public void Debug(object message)
    {
        UnityEngine.Debug.Log(message);
    }
    public void Debug(object message, Exception exception)
    {
        UnityEngine.Debug.Log(message);
        UnityEngine.Debug.LogException(exception);
    }
    public void Info(object message)
    {
        Debug(message);
    }
    public void Info(object message, Exception exception)
    {
        Debug(message, exception);
    }
    public void Warn(object message)
    {
        UnityEngine.Debug.LogWarning(message);
    }
    public void Warn(object message, Exception exception)
    {
        UnityEngine.Debug.LogWarning(message);
        UnityEngine.Debug.LogException(exception);
    }
    public void Error(object message)
    {
        UnityEngine.Debug.LogError(message);
    }
    public void Error(object message, Exception exception)
    {
        UnityEngine.Debug.LogError(message);
        UnityEngine.Debug.LogException(exception);
    }
    public void Fatal(object message)
    {
        Error(message);
    }
    public void Fatal(object message, Exception exception)
    {
        Error(message, exception);
    }
}
