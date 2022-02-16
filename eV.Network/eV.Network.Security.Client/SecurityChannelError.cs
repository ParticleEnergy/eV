// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

namespace eV.Network.Security.Client;

public enum SecurityChannelError
{
    TcpClientIsNull = 0,
    TcpClientNotConnect = 1,
    SslStreamIsNull = 2,
    SslStreamIoError = 3
}
