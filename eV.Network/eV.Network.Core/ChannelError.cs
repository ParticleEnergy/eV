// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.EasyLog;
namespace eV.Network.Core;

public static class ChannelError
{
    public enum ErrorCode
    {
        SocketIsNull = 0,
        SocketNotConnect = 1,
        SocketError = 2,
        SocketBytesTransferredIsZero = 3, // 检查远程主机是否关闭了连接。
        TcpClientIsNull = 4,
        TcpClientNotConnect = 5,
        SslStreamIsNull = 6,
        SslStreamIoError = 7
    }

    public static void Error(ErrorCode channelErrorCode, Action action)
    {
        switch (channelErrorCode)
        {
            case ErrorCode.SocketIsNull:
                action();
                break;
            case ErrorCode.SocketNotConnect:
                action();
                break;
            case ErrorCode.SocketError:
                action();
                break;
            case ErrorCode.SocketBytesTransferredIsZero:
                action();
                break;
            case ErrorCode.TcpClientIsNull:
                action();
                break;
            case ErrorCode.TcpClientNotConnect:
                action();
                break;
            case ErrorCode.SslStreamIsNull:
                action();
                break;
            case ErrorCode.SslStreamIoError:
                action();
                break;
            default:
                Logger.Error("ChannelError not found");
                break;
        }
    }
}
