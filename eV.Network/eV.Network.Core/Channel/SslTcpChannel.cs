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
    private readonly SocketAsyncEventArgs _disconnectSocketAsyncEventArgs;
    private readonly byte[] _receiveBuffer;

    #endregion

    #region Public Setting

    private readonly bool _checkCertificateRevocation;

    #endregion

    #region Server Setting

    private readonly bool? _clientCertificateRequired;
    private readonly X509Certificate2? _x509Certificate2;

    #endregion

    #region Client Setting

    private readonly string? _targetHost;
    private readonly X509CertificateCollection? _x509CertificateCollection;

    #endregion

    #region Constructor

    public SslTcpChannel(int receiveBufferSize, X509Certificate2 x509Certificate2, bool clientCertificateRequired, bool checkCertificateRevocation)
    {
        ChannelId = Guid.NewGuid().ToString();
        ChannelState = RunState.Off;

        // Setting
        _x509Certificate2 = x509Certificate2;
        _clientCertificateRequired = clientCertificateRequired;
        _checkCertificateRevocation = checkCertificateRevocation;

        // Completed
        SocketAsyncEventArgsCompleted socketAsyncEventArgsCompleted = new();
        socketAsyncEventArgsCompleted.ProcessDisconnect += ProcessDisconnect;
        // Receive
        _receiveBuffer = new byte[receiveBufferSize];

        // Disconnect
        _disconnectSocketAsyncEventArgs = new SocketAsyncEventArgs();
        _disconnectSocketAsyncEventArgs.Completed += socketAsyncEventArgsCompleted.OnCompleted;
        _disconnectSocketAsyncEventArgs.DisconnectReuseSocket = false;
    }

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
        socketAsyncEventArgsCompleted.ProcessDisconnect += ProcessDisconnect;
        // Receive
        _receiveBuffer = new byte[receiveBufferSize];

        // Disconnect
        _disconnectSocketAsyncEventArgs = new SocketAsyncEventArgs();
        _disconnectSocketAsyncEventArgs.Completed += socketAsyncEventArgsCompleted.OnCompleted;
        _disconnectSocketAsyncEventArgs.DisconnectReuseSocket = false;
    }

    #endregion


    #region Operate

    public async void Open(Socket socket)
    {
        if (ChannelState == RunState.On)
        {
            Logger.Warn("The channel is already turned on");
            return;
        }

        try
        {
            Init(socket);

            if (!await Authenticate())
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
            Close();
        }
    }

    public void Close()
    {
        if (ChannelState == RunState.Off)
            return;
        ChannelState = RunState.Off;
        try
        {
            _sslStream?.Close();
            _sslStream?.Dispose();

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
        _sslStream = new SslStream(new NetworkStream(socket), false, RemoteCertificateValidationCallback);
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

        if (!_sslStream.CanRead)
        {
            ChannelError.Error(ChannelError.ErrorCode.SslStreamIoError, Close);
            return false;
        }

        _sslStream.BeginRead(_receiveBuffer, 0, _receiveBuffer.Length, ReceiveCallback, null);
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
            if (bytesRead > 0)
            {
                Receive?.Invoke(_receiveBuffer.Skip(0).Take(bytesRead).ToArray());
                LastReceiveDateTime = DateTime.Now;

                StartReceive();
            }
            else
            {
                Close();
            }
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

    #region Validation

    private async Task<bool> Authenticate()
    {
        if (_sslStream == null)
        {
            Close();
            return false;
        }

        try
        {
            if (_targetHost != null && !_targetHost.Equals("") && _x509CertificateCollection != null)
            {
                await _sslStream.AuthenticateAsClientAsync(_targetHost, _x509CertificateCollection, SslProtocols.Tls12, _checkCertificateRevocation);
            }
            else
            {
                await _sslStream.AuthenticateAsServerAsync(_x509Certificate2!, _clientCertificateRequired ?? false, SslProtocols.Tls12, _checkCertificateRevocation);
            }

            return _sslStream.IsAuthenticated && _sslStream.IsEncrypted;
        }
        catch (AuthenticationException e)
        {
            Logger.Error(e.Message, e);
            if (e.InnerException != null)
            {
                Logger.Error($"Inner exception message: {e.InnerException.Message}", e.InnerException);
            }

            Close();
            return false;
        }
    }

    private bool RemoteCertificateValidationCallback(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
    {
        // 如果启用了调试模式，则允许所有证书
        if (Logger.IsDebug())
            return true;

        // 如果有任何策略错误，则拒绝证书
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            Logger.Error($"Certificate error: {sslPolicyErrors}");
            return false;
        }

        // 如果证书为 null，则拒绝证书
        if (certificate == null)
        {
            Logger.Error("Certificate is null");
            return false;
        }

        // 如果证书链为 null，则拒绝证书
        if (chain == null)
        {
            Logger.Error("Certificate chain is null");
            return false;
        }

        // 检查证书是否被吊销
        if (chain.ChainStatus.Any(status => status.Status == X509ChainStatusFlags.RevocationStatusUnknown || status.Status == X509ChainStatusFlags.Revoked))
        {
            Logger.Error("Certificate is revoked or revocation status is unknown");
            return false;
        }

        try
        {
            // 检查证书是否有效
            chain.Build(new X509Certificate2(certificate));
        }
        catch (Exception ex)
        {
            Logger.Error($"Error validating certificate: {ex.Message}");
            return false;
        }

        // 检查证书是否过期
        if (DateTime.Now < DateTime.Parse(certificate.GetEffectiveDateString()) || DateTime.Now > DateTime.Parse(certificate.GetExpirationDateString()))
        {
            Logger.Error("Certificate is expired");
            return false;
        }

        // 检查证书是否颁发给正确的主机名
        if (!certificate.Subject.Contains($"CN={_targetHost}", StringComparison.OrdinalIgnoreCase))
        {
            Logger.Error($"Certificate is not issued to {_targetHost}");
            return false;
        }

        // 验证证书链
        bool chainIsValid = true;
        for (int i = 0; i < chain.ChainElements.Count - 1; i++)
        {
            var current = chain.ChainElements[i];
            var issuer = chain.ChainElements[i + 1];

            // 检查颁发者和主题名称是否匹配
            if (current.Certificate.Issuer.Equals(issuer.Certificate.Subject, StringComparison.OrdinalIgnoreCase)) continue;
            chainIsValid = false;
            break;
        }

        if (!chainIsValid)
        {
            Logger.Error("Certificate chain validation failed");
            return false;
        }

        // 证书验证成功
        return true;
    }

    #endregion
}
