// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Net;

namespace eV.Network.Core.Interface;

public interface IUdpChannel : IChannel
{
    public Action<byte[]?, EndPoint?>? Receive { get; set; }
    public bool SendBroadcast(byte[] data);
    public bool SendMulticast(byte[] data);
    public bool Send(byte[] data, EndPoint destEndPoint);
}
