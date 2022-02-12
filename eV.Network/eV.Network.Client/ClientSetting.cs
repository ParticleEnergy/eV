// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Net.Sockets;
namespace eV.Network.Client;

public class ClientSetting
{
    public int ReceiveBufferSize { get; set; } = DefaultSetting.ReceiveBufferSize;
    #region Socket
    public string Address { get; set; } = DefaultSetting.Address;
    public int Port { get; set; } = DefaultSetting.Port;
    public SocketType SocketType { get; set; } = DefaultSetting.SocketType;
    public ProtocolType ProtocolType { get; set; } = DefaultSetting.ProtocolType;
    #endregion
}
