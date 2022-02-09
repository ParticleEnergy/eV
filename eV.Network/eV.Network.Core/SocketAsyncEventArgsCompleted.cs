// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Net.Sockets;
using eV.EasyLog;
namespace eV.Network.Core
{
    public delegate void AsyncCompleted(SocketAsyncEventArgs socketAsyncEventArgs);

    public class SocketAsyncEventArgsCompleted
    {
        public event AsyncCompleted? ProcessAccept;
        public event AsyncCompleted? ProcessConnect;
        public event AsyncCompleted? ProcessDisconnect;
        public event AsyncCompleted? ProcessReceive;
        public event AsyncCompleted? ProcessSend;

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
                    break;
                case SocketAsyncOperation.ReceiveMessageFrom:
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend?.Invoke(socketAsyncEventArgs);
                    break;
                case SocketAsyncOperation.SendPackets:
                    break;
                case SocketAsyncOperation.SendTo:
                    break;
                default:
                    Logger.Warn(
                        $"{socketAsyncEventArgs.LastOperation} not found in socketAsyncEventArgs.LastOperation"
                    );
                    break;
            }
        }
    }
}
