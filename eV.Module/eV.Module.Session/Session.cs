// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Collections;
using eV.Module.EasyLog;
using eV.Module.Routing;
using eV.Module.Routing.Interface;
using eV.Network.Core;
using eV.Network.Core.Interface;

namespace eV.Module.Session;

public sealed class Session : ISession
{
    private readonly ITcpChannel _channel;
    private readonly DataParser _dataParser = new();

    public Session(ITcpChannel channel)
    {
        SessionState = SessionState.Free;
        SessionData = new Hashtable();
        // Channel
        _channel = channel;
        _channel.Receive = Receive;
        _channel.CloseCompleted += ChannelClose;
    }

    #region Channel

    private void ChannelClose(ITcpChannel channel)
    {
        Release();
    }

    #endregion

    #region Event

    public event SessionEvent? OnActivate;
    public event SessionEvent? OnRelease;

    #endregion

    #region Action

    public Func<string, byte[], bool>? SendAction;
    public Action<string, string, byte[]>? SendGroupAction;
    public Action<string, byte[]>? SendBroadcastAction;
    public Func<string, string, bool>? JoinGroupAction;
    public Func<string, string, bool>? LeaveGroupAction;

    #endregion

    #region SessionId Property

    public string? SessionId
    {
        get => _sessionId;
        set { }
    }

    private string? _sessionId;

    public Hashtable SessionData { get; }

    public SessionState SessionState { get; private set; }

    #endregion

    #region Group Property

    public Dictionary<string, string> Group
    {
        get => _group;
        set { }
    }

    private readonly Dictionary<string, string> _group = new();

    #endregion

    #region Time

    public DateTime? ConnectedDateTime
    {
        get => _channel.ConnectedDateTime;
        set { }
    }

    public DateTime? LastActiveDateTime
    {
        get => _channel.LastSendDateTime;
        set { }
    }

    #endregion

    #region Operate

    public void Occupy()
    {
        SessionState = SessionState.Occupy;
    }

    public void Activate(string sessionId)
    {
        if (SessionState == SessionState.Active)
            return;
        _sessionId = sessionId;
        SessionState = SessionState.Active;
        OnActivate?.Invoke(this);
    }

    public void Shutdown()
    {
        if (SessionState == SessionState.Free)
            return;
        if (_channel.ChannelState == RunState.On)
            _channel.Close();
        Release();
    }

    private void Release()
    {
        OnRelease?.Invoke(this);
        foreach (KeyValuePair<string, string> g in _group)
            LeaveGroup(g.Value);
        _sessionId = null;
        _group.Clear();
        _dataParser.Reset();
        SessionData.Clear();
        SessionState = SessionState.Free;
    }

    #endregion


    #region IO

    private static KeyValuePair<string, byte[]?> GetSendData<T>(T data)
    {
        try
        {
            string name = Dispatch.GetSendMessageName(typeof(T));
            if (name.Equals(""))
                return new KeyValuePair<string, byte[]?>(name, null);
            Packet packet = new();
            packet.SetName(name);
            packet.SetContent(Serializer.Serialize(data));
            return new KeyValuePair<string, byte[]?>(name, Package.Pack(packet));
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return new KeyValuePair<string, byte[]?>("", null);
        }
    }

    public bool Send(byte[] data)
    {
        try
        {
            return _channel.ChannelState == RunState.On && _channel.Send(data);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return false;
        }
    }

    public bool Send<T>(T data)
    {
        try
        {
            KeyValuePair<string, byte[]?> result = GetSendData(data);
            if (result.Value == null || !Send(result.Value))
                return false;
            SessionDebug.DebugSend(SessionId, result.Key, data);
            return true;
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return false;
        }
    }

    public bool Send<T>(string sessionId, T data)
    {
        if (_sessionId is null or "")
        {
            Logger.Warn("SendBySessionId needs to activate the session");
            return false;
        }

        try
        {
            KeyValuePair<string, byte[]?> result = GetSendData(data);
            if (result.Value == null || SendAction == null || SendAction.Invoke(sessionId, result.Value))
                return false;
            SessionDebug.DebugSend(SessionId, sessionId, result.Key, data);
            return true;
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return false;
        }
    }

    public void SendGroup<T>(string groupId, T data)
    {
        if (_sessionId is null or "")
        {
            Logger.Warn("SendGroup needs to activate the session");
            return;
        }

        try
        {
            KeyValuePair<string, byte[]?> result = GetSendData(data);
            if (result.Value == null || SendGroupAction == null)
                return;
            SendGroupAction?.Invoke(_sessionId, groupId, result.Value);
            SessionDebug.DebugSendGroup(SessionId, groupId, result.Key, data);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }

    public void SendBroadcast<T>(T data)
    {
        if (_sessionId is null or "")
        {
            Logger.Warn("SendBroadcast needs to activate the session");
            return;
        }

        try
        {
            KeyValuePair<string, byte[]?> result = GetSendData(data);
            if (result.Value == null || SendBroadcastAction == null)
                return;
            SendBroadcastAction?.Invoke(_sessionId, result.Value);
            SessionDebug.DebugSendBroadcast(SessionId, result.Key, data);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }

    private async void Receive(byte[]? data)
    {
        try
        {
            if (data == null)
                return;

            List<Packet> packets = _dataParser.Parsing(data);
            if (packets is not { Count: > 0 })
                return;

            foreach (Packet? packet in packets)
            {
                KeyValuePair<string, object> result = await Dispatch.Dispense(this, packet);
                SessionDebug.DebugReceive(SessionId, result.Key, result.Value);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            Shutdown();
        }
    }

    #endregion

    #region Group

    public bool JoinGroup(string groupId)
    {
        if (JoinGroupAction == null || _sessionId is null or "")
            return false;
        if (!JoinGroupAction.Invoke(groupId, _sessionId))
            return false;
        _group[groupId] = _sessionId;
        return true;
    }

    public bool LeaveGroup(string groupId)
    {
        if (LeaveGroupAction == null || _sessionId is null or "")
            return false;
        if (!LeaveGroupAction.Invoke(groupId, _sessionId))
            return false;
        _group.Remove(groupId);
        return true;
    }

    #endregion
}
