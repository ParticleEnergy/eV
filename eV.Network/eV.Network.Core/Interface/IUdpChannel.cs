// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Net;
namespace eV.Network.Core.Interface;

public interface IUdpChannel : IChannel
{
    public bool SendBroadcast(byte[] data);

    public bool SendMulticast(byte[] data);

    public bool Send(byte[] data, EndPoint destEndPoint);
}
