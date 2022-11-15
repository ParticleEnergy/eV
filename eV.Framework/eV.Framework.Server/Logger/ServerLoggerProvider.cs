// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace eV.Framework.Server.Logger;

public class ServerLoggerProvider : ILoggerProvider
{
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new ServerLogger(categoryName);
    }
}
