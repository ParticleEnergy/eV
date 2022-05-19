// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

namespace eV.Network.Udp;

internal static class DefaultSetting
{
    public const int ListenPort = 8888;
    public const string Localhost = "127.0.0.1";
    public const string MultiCastHost = "234.66.66.66";
    public const int MulticastTimeToLive = 10;
    public const bool MulticastLoopback = false;

    public const int MaxConcurrentSend = 100;
    public const int ReceiveBufferSize = 2048;
}
