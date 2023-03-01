// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Text;
using eV.Module.EasyLog;

namespace eV.Module.Storage.Redis;

public class RedisLogger : TextWriter
{
    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(char value)
    {
        Logger.Debug(value.ToString());
    }

    public override void Write(char[] buffer, int index, int count)
    {
        Logger.Debug(new string(buffer, index, count));
    }

    public override void Write(string? value)
    {
        if (value == null)
            return;
        Logger.Debug(value);
    }

    public override void WriteLine(string? value)
    {
        if (value == null)
            return;
        Logger.Debug(value);
    }

    public override void Flush()
    {
        Logger.Debug("Flush called");
    }

    public override void Close()
    {
        Logger.Debug("Close called");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Logger.Debug("Dispose called");
        }

        base.Dispose(disposing);
    }
}
