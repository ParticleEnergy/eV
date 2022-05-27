// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Module.EasyLog.Interface;

public interface ILog
{
    void Debug(object message);
    void Debug(object message, Exception exception);
    void Info(object message);
    void Info(object message, Exception exception);
    void Warn(object message);
    void Warn(object message, Exception exception);
    void Error(object message);
    void Error(object message, Exception exception);
    void Fatal(object message);
    void Fatal(object message, Exception exception);
}
