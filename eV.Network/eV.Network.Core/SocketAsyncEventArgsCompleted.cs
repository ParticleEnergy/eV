// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Net.Sockets;
using eV.Module.EasyLog;

namespace eV.Network.Core;

public class SocketAsyncEventArgsCompleted
{
    public event AsyncCompletedEvent? ProcessAccept;
    public event AsyncCompletedEvent? ProcessConnect;
    public event AsyncCompletedEvent? ProcessDisconnect;
    public event AsyncCompletedEvent? ProcessReceive;
    public event AsyncCompletedEvent? ProcessSend;
    public event AsyncCompletedEvent? ProcessReceiveFrom;
    public event AsyncCompletedEvent? ProcessSendTo;

    public void OnCompleted(object? sender, SocketAsyncEventArgs socketAsyncEventArgs)
    {
        switch (socketAsyncEventArgs.LastOperation)
        {
            case SocketAsyncOperation.None:
                break;
            case SocketAsyncOperation.Accept:
                ProcessAccept?.Invoke(socketAsyncEventArgs);
                break;
            case SocketAsyncOperation.Connect:
                ProcessConnect?.Invoke(socketAsyncEventArgs);
                break;
            case SocketAsyncOperation.Disconnect:
                ProcessDisconnect?.Invoke(socketAsyncEventArgs);
                break;
            case SocketAsyncOperation.Receive:
                ProcessReceive?.Invoke(socketAsyncEventArgs);
                break;
            case SocketAsyncOperation.ReceiveFrom:
                ProcessReceiveFrom?.Invoke(socketAsyncEventArgs);
                break;
            case SocketAsyncOperation.ReceiveMessageFrom:
                break;
            case SocketAsyncOperation.Send:
                ProcessSend?.Invoke(socketAsyncEventArgs);
                break;
            case SocketAsyncOperation.SendPackets:
                break;
            case SocketAsyncOperation.SendTo:
                ProcessSendTo?.Invoke(socketAsyncEventArgs);
                break;
            default:
                Logger.Warn(
                    $"{socketAsyncEventArgs.LastOperation} not found in socketAsyncEventArgs.LastOperation"
                );
                break;
        }
    }
}
