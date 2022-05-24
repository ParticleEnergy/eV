// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using eV.Module.EasyLog;
using eV.Network.Core.Interface;
namespace eV.Network.Core.Channel;

public class TcpSecurityChannel : ITcpChannel
{
    #region Event
    public event TcpChannelEvent? OpenCompleted;
    public event TcpChannelEvent? CloseCompleted;
    #endregion
    #region Time
    public DateTime? ConnectedDateTime
    {
        get;
        private set;
    }
    public DateTime? LastReceiveDateTime
    {
        get;
        private set;
    }
    public DateTime? LastSendDateTime
    {
        get;
        private set;
    }
    #endregion
    #region public
    public RunState ChannelState
    {
        get;
        private set;
    }
    public string ChannelId
    {
        get;
    }
    public EndPoint? RemoteEndPoint
    {
        get;
        private set;
    }
    #endregion
    #region Resource
    private SslStream? _sslStream;
    private TcpClient? _tcpClient;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _task;
    private readonly byte[] _receiveBuffer;
    private readonly SslProtocols _sslProtocols;
    private readonly string _targetHost;
    private readonly string _certFile;
    #endregion
    public TcpSecurityChannel(string targetHost, string certFile, SslProtocols sslProtocols, int receiveBufferSize)
    {
        ChannelId = Guid.NewGuid().ToString();
        ChannelState = RunState.Off;
        _targetHost = targetHost;
        _certFile = certFile;
        _sslProtocols = sslProtocols;

        _receiveBuffer = new byte[receiveBufferSize];
    }


    #region Operate
    public void Open(TcpClient tcpClient)
    {
        if (ChannelState == RunState.On)
        {
            Logger.Warn("The channel is already turned on");
            return;
        }
        try
        {
            Init(tcpClient);

            if (!Authenticate())
                return;

            OpenCompleted?.Invoke(this);
            Logger.Info($"Channel {ChannelId} {RemoteEndPoint} open");

            _task!.Start();
            Logger.Info($"Channel {ChannelId} {RemoteEndPoint} start receive");
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
            if (_tcpClient is { Connected: true })
                _tcpClient.Client.Shutdown(SocketShutdown.Both);
            else
                Release();
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }
#if !NETSTANDARD2_0_OR_GREATER
    private async void Release()
    {
        if (_sslStream != null)
            await _sslStream.ShutdownAsync();
#else
    private void Release()
    {
#endif
        _tcpClient?.Close();
        _sslStream?.Close();
        _cancellationTokenSource?.Cancel();
        _sslStream = null;
        _tcpClient = null;

        Array.Clear(_receiveBuffer, 0, _receiveBuffer.Length);

        Logger.Info($"Channel {ChannelId} {RemoteEndPoint} close");
        CloseCompleted?.Invoke(this);
    }

    private void Init(TcpClient tcpClient)
    {
        _tcpClient = tcpClient;
        _sslStream = new SslStream(tcpClient.GetStream(), false, ValidateServerCertificate, null);
        _cancellationTokenSource = new CancellationTokenSource();
        ChannelState = RunState.On;
        ConnectedDateTime = DateTime.Now;
        RemoteEndPoint = tcpClient.Client.RemoteEndPoint;

        _task = new Task(delegate
        {
            if (!StartReceive())
                Logger.Debug($"Channel {ChannelId} receive shutdown");
        }, _cancellationTokenSource!.Token);

    }
    #endregion

    #region IO
    private bool StartReceive()
    {
        while (_cancellationTokenSource is { IsCancellationRequested: false })
        {
            if (ChannelState == RunState.Off)
                return false;
            if (_tcpClient == null)
            {
                ChannelError.Error(ChannelError.ErrorCode.TcpClientIsNull, Close);
                return false;
            }
            if (!_tcpClient.Connected)
            {
                ChannelError.Error(ChannelError.ErrorCode.TcpClientNotConnect, Close);
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
            while (true)
            {
                int bytes = _sslStream.ReadAsync(_receiveBuffer, 0, _receiveBuffer.Length).Result;
                if (bytes <= 0)
                    break;
                Receive?.Invoke(_receiveBuffer.Skip(0).Take(bytes).ToArray());
                LastReceiveDateTime = DateTime.Now;
            }
            Array.Clear(_receiveBuffer, 0, _receiveBuffer.Length);
        }
        return true;
    }
    public Action<byte[]?>? Receive { get; set; }
    public bool Send(byte[] data)
    {
        if (ChannelState == RunState.Off)
            return false;
        if (_tcpClient == null)
        {
            ChannelError.Error(ChannelError.ErrorCode.TcpClientIsNull, Close);
            return false;
        }
        if (!_tcpClient.Connected)
        {
            ChannelError.Error(ChannelError.ErrorCode.TcpClientNotConnect, Close);
            return false;
        }
        if (_sslStream == null)
        {
            ChannelError.Error(ChannelError.ErrorCode.SslStreamIsNull, Close);
            return false;
        }
        if (!_sslStream.CanWrite)
        {
            ChannelError.Error(ChannelError.ErrorCode.SslStreamIoError, Close);
            return false;
        }
        _sslStream.WriteAsync(data, 0, data.Length);
        _sslStream.Flush();
        LastSendDateTime = DateTime.Now;
        return true;
    }
    #endregion

    #region Auth
    private bool ValidateServerCertificate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
    {
        if (sslPolicyErrors == SslPolicyErrors.None)
            return true;

        Logger.Error($"Channel {ChannelId} SslPolicyErrors [{sslPolicyErrors}]");
        return false;
    }

    private bool Authenticate()
    {
        if (_sslStream == null)
        {
            Close();
            return false;
        }
        X509CertificateCollection certs = new();
        X509Certificate cert = X509Certificate.CreateFromCertFile(_certFile);
        certs.Add(cert);
        try
        {
            _sslStream?.AuthenticateAsClient(_targetHost, certs, _sslProtocols, false);
            return true;
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
    #endregion
}
