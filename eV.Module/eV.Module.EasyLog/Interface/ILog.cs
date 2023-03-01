// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Module.EasyLog.Interface;

public interface ILog
{
    public bool IsDebug();
    public void Debug(object message);
    public void Debug(object message, Exception exception);
    public void Info(object message);
    public void Info(object message, Exception exception);
    public void Warn(object message);
    public void Warn(object message, Exception exception);
    public void Error(object message);
    public void Error(object message, Exception exception);
    public void Fatal(object message);
    public void Fatal(object message, Exception exception);
}
