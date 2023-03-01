// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Security.Authentication;

namespace eV.Framework.Unity;

internal static class DefaultSetting
{
    public const string Logo = @"
      _    __   __  __      _ __
  ___| |  / /  / / / /___  (_) /___  __
 / _ \ | / /  / / / / __ \/ / __/ / / /
/  __/ |/ /  / /_/ / / / / / /_/ /_/ /
\___/|___/   \____/_/ /_/_/\__/\__, /
                              /____/
";

    public const string Host = "0.0.0.0";
    public const int Port = 8888;
    public const int ReceiveBufferSize = 2014;

    public const bool TcpKeepAlive = true;

    public const SslProtocols SslProtocols = (SslProtocols)192;
    public const string TargetHost = "eV";
}
