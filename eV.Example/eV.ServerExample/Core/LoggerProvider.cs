// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using Microsoft.Extensions.Logging;
namespace eV.ServerExample.Core;

public class LoggerProvider : ILoggerProvider
{
    public void Dispose()
    {

    }

    public ILogger CreateLogger(string _)
    {
        return new Logger();
    }
}
