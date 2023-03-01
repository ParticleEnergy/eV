// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Net.Sockets;
using eV.Network.Core.Interface;

namespace eV.Network.Core;

public delegate bool AsyncCompletedEvent(SocketAsyncEventArgs socketAsyncEventArgs);

public delegate void TcpChannelEvent(ITcpChannel channel);

public delegate void UdpChannelEvent(IUdpChannel channel);
