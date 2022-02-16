// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Net.Sockets;
namespace eV.Network.Security.Client;

public class Client
{
    public Client()
    {
        TcpClient tcpClient = new();
        tcpClient.GetStream();
    }
}
