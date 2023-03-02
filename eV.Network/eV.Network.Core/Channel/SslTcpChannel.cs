// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using eV.Module.EasyLog;
using eV.Network.Core.Interface;

namespace eV.Network.Core.Channel;

public class SslTcpChannel : ITcpChannel
{
    #region Event

    public event TcpChannelEvent? OpenCompleted;
    public event TcpChannelEvent? CloseCompleted;

    #endregion

    #region Public

    public RunState ChannelState { get; private set; }
    public string ChannelId { get; }
    public EndPoint? RemoteEndPoint { get; private set; }

    #endregion

    #region Time

    public DateTime? ConnectedDateTime { get; private set; }
    public DateTime? LastReceiveDateTime { get; private set; }
    public DateTime? LastSendDateTime { get; private set; }

    #endregion

    #region Resource

    private Socket? _socket;
    private SslStream? _sslStream;
    private readonly SocketAsyncEventArgs _receiveSocketAsyncEventArgs;
    private readonly SocketAsyncEventArgs _disconnectSocketAsyncEventArgs;
    private readonly byte[] _receiveBuffer;

    #endregion

    #region Setting

    private readonly string _targetHost;
    private readonly X509CertificateCollection _x509CertificateCollection;
    private readonly bool _checkCertificateRevocation;

    #endregion

    public SslTcpChannel(int receiveBufferSize, string targetHost, X509CertificateCollection x509CertificateCollection, bool checkCertificateRevocation)
    {
        ChannelId = Guid.NewGuid().ToString();
        ChannelState = RunState.Off;

        // Setting
        _targetHost = targetHost;
        _x509CertificateCollection = x509CertificateCollection;
        _checkCertificateRevocation = checkCertificateRevocation;

        // Completed
        SocketAsyncEventArgsCompleted socketAsyncEventArgsCompleted = new();
        socketAsyncEventArgsCompleted.ProcessReceive += ProcessReceive;
        socketAsyncEventArgsCompleted.ProcessDisconnect += ProcessDisconnect;
        // Receive
        _receiveBuffer = new byte[receiveBufferSize];
        _receiveSocketAsyncEventArgs = new SocketAsyncEventArgs();
        _receiveSocketAsyncEventArgs.Completed += socketAsyncEventArgsCompleted.OnCompleted;
        _receiveSocketAsyncEventArgs.SetBuffer(_receiveBuffer, 0, receiveBufferSize);

        // Disconnect
        _disconnectSocketAsyncEventArgs = new SocketAsyncEventArgs();
        _disconnectSocketAsyncEventArgs.Completed += socketAsyncEventArgsCompleted.OnCompleted;
        _disconnectSocketAsyncEventArgs.DisconnectReuseSocket = false;
    }

    #region Operate

    public void Open(Socket socket)
    {
        if (ChannelState == RunState.On)
        {
            Logger.Warn("The channel is already turned on");
            return;
        }

        try
        {
            Init(socket);

            if (!Authenticate())
            {
                Close();
                return;
            }

            OpenCompleted?.Invoke(this);
            Logger.Info($"Channel {ChannelId} {RemoteEndPoint} open");
            Logger.Info(StartReceive()
                ? $"Channel {ChannelId} {RemoteEndPoint} start receive"
                : $"Channel {ChannelId} {RemoteEndPoint} start receive failed");
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }

    public void Close()
    {
        if (ChannelState == RunState.Off)
            return;
        ChannelState = RunState.Off;
        try
        {
            _sslStream?.ShutdownAsync().Wait();
            _sslStream?.Close();

            if (_socket is { Connected: true })
            {
                _socket.Shutdown(SocketShutdown.Both);
                if (!_socket.DisconnectAsync(_disconnectSocketAsyncEventArgs))
                    ProcessDisconnect(_disconnectSocketAsyncEventArgs);
            }
            else
            {
                Release();
            }
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }

    /// <summary>
    ///     释放资源
    /// </summary>
    private void Release()
    {
        try
        {
            _socket?.Close();
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
        finally
        {
            _socket = null;
            _sslStream = null;
            _receiveSocketAsyncEventArgs.UserToken = null;
            Array.Clear(_receiveBuffer, 0, _receiveBuffer.Length);

            Logger.Info($"Channel {ChannelId} {RemoteEndPoint} close");
            CloseCompleted?.Invoke(this);
        }
    }

    /// <summary>
    ///     重置Channel
    /// </summary>
    private void Init(Socket socket)
    {
        _socket = socket;
        _sslStream = new SslStream(new NetworkStream(socket));
        ChannelState = RunState.On;
        ConnectedDateTime = DateTime.Now;
        RemoteEndPoint = _socket?.RemoteEndPoint;
    }

    #endregion

    #region IO

    private bool StartReceive()
    {
        if (ChannelState == RunState.Off)
            return false;
        if (_socket == null)
        {
            ChannelError.Error(ChannelError.ErrorCode.SocketIsNull, Close);
            return false;
        }

        if (!_socket.Connected)
        {
            ChannelError.Error(ChannelError.ErrorCode.SocketNotConnect, Close);
            return false;
        }

        if (_sslStream == null)
        {
            ChannelError.Error(ChannelError.ErrorCode.SslStreamIsNull, Close);
            return false;
        }

        if (_receiveSocketAsyncEventArgs.Buffer == null)
        {
            ChannelError.Error(ChannelError.ErrorCode.SslStreamIoError, Close);
            return false;
        }

        _sslStream.BeginRead(_receiveSocketAsyncEventArgs.Buffer, _receiveSocketAsyncEventArgs.Offset, _receiveSocketAsyncEventArgs.Count, ReceiveCallback, null);
        return true;
    }

    public Action<byte[]?>? Receive { get; set; }

    public bool Send(byte[] data)
    {
        if (ChannelState == RunState.Off)
            return false;
        if (_socket == null)
        {
            ChannelError.Error(ChannelError.ErrorCode.SocketIsNull, Close);
            return false;
        }

        if (!_socket.Connected)
        {
            ChannelError.Error(ChannelError.ErrorCode.SocketNotConnect, Close);
            return false;
        }

        if (_sslStream == null)
        {
            ChannelError.Error(ChannelError.ErrorCode.SslStreamIsNull, Close);
            return false;
        }

        try
        {
            lock (_sslStream)
            {
                _sslStream.BeginWrite(data, 0, data.Length, SendCallback, null);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return false;
        }

        return true;
    }

    #endregion

    #region Process

    private bool ProcessReceive(SocketAsyncEventArgs socketAsyncEventArgs)
    {
        try
        {
            if (_socket == null)
            {
                ChannelError.Error(ChannelError.ErrorCode.SocketIsNull, Close);
                return false;
            }

            if (!_socket.Connected)
            {
                ChannelError.Error(ChannelError.ErrorCode.SocketNotConnect, Close);
                return false;
            }

            if (socketAsyncEventArgs.BytesTransferred == 0)
            {
                ChannelError.Error(ChannelError.ErrorCode.SocketBytesTransferredIsZero, Close);
                return false;
            }

            if (socketAsyncEventArgs.UserToken != null)
            {
                Receive?.Invoke((byte[])socketAsyncEventArgs.UserToken);
                LastReceiveDateTime = DateTime.Now;
            }
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            Close();
            return false;
        }

        return true;
    }

    private bool ProcessDisconnect(SocketAsyncEventArgs socketAsyncEventArgs)
    {
        Logger.Info($"Channel {ChannelId} {RemoteEndPoint} disconnect");
        Release();
        return true;
    }

    #endregion

    #region Callback

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            if (_socket == null)
            {
                ChannelError.Error(ChannelError.ErrorCode.SocketIsNull, Close);
                return;
            }

            if (!_socket.Connected)
            {
                ChannelError.Error(ChannelError.ErrorCode.SocketNotConnect, Close);
                return;
            }

            if (_sslStream == null)
            {
                ChannelError.Error(ChannelError.ErrorCode.SslStreamIsNull, Close);
                return;
            }

            int bytesRead = _sslStream.EndRead(ar);
            if (bytesRead > 0 && _receiveSocketAsyncEventArgs.Buffer != null)
            {
                byte[] data = new byte[bytesRead];
                Buffer.BlockCopy(_receiveSocketAsyncEventArgs.Buffer, _receiveSocketAsyncEventArgs.Offset, data, 0, bytesRead);
                _receiveSocketAsyncEventArgs.UserToken = data;
                _receiveSocketAsyncEventArgs.SetBuffer(_receiveSocketAsyncEventArgs.Offset, _receiveSocketAsyncEventArgs.Count);
                ProcessReceive(_receiveSocketAsyncEventArgs);
            }

            StartReceive();
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            Close();
        }
    }

    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            if (_socket == null)
            {
                ChannelError.Error(ChannelError.ErrorCode.SocketIsNull, Close);
                return;
            }

            if (!_socket.Connected)
            {
                ChannelError.Error(ChannelError.ErrorCode.SocketNotConnect, Close);
                return;
            }

            if (_sslStream == null)
            {
                ChannelError.Error(ChannelError.ErrorCode.SslStreamIsNull, Close);
                return;
            }

            _sslStream.EndWrite(ar);

            LastSendDateTime = DateTime.Now;
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }

    #endregion

    private bool Authenticate()
    {
        if (_sslStream == null)
        {
            Close();
            return false;
        }

        try
        {
            _sslStream.AuthenticateAsClient(_targetHost, _x509CertificateCollection, SslProtocols.Tls12, _checkCertificateRevocation);

            return _sslStream.IsAuthenticated && _sslStream.IsEncrypted;
        }
        catch (AuthenticationException e)
        {
            Logger.Error(e.Message, e);
            if (e.InnerException != null)
                Logger.Error(e.InnerException.Message, e.InnerException);
            Close();
            return false;
        }
    }
}
