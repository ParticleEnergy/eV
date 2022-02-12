// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

namespace eV.Network.Core;

public enum ChannelError
{
    SocketIsNull = 0,
    SocketNotConnect = 1,
    SocketError = 2,
    SocketBytesTransferredIsZero = 3 // 检查远程主机是否关闭了连接。
}
