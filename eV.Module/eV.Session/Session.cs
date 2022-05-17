// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Collections;
using eV.EasyLog;
using eV.Network.Core;
using eV.Network.Core.Interface;
using eV.Routing;
using eV.Routing.Interface;
namespace eV.Session;

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
        set {}
    }
    private string? _sessionId;

    public Hashtable SessionData { get; }

    public SessionState SessionState { get; private set; }
    #endregion

    #region Group Property
    public Dictionary<string, string> Group
    {
        get => _group;
        set {}
    }
    private readonly Dictionary<string, string> _group = new();
    #endregion
    #region Time
    public DateTime? ConnectedDateTime
    {
        get => _channel.ConnectedDateTime;
        set {}
    }
    public DateTime? LastActiveDateTime
    {
        get => _channel.LastSendDateTime;
        // if (_channel.LastSendDateTime == null && _channel.LastReceiveDateTime == null)
        //     return _channel.ConnectedDateTime;
        // if (_channel.LastSendDateTime != null && _channel.LastReceiveDateTime != null)
        //     return _channel.LastSendDateTime >= _channel.LastReceiveDateTime ? _channel.LastSendDateTime : _channel.LastReceiveDateTime;
        // return _channel.LastSendDateTime ?? _channel.LastReceiveDateTime;
        set {}
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
        OnActivate?.Invoke(this);
        SessionState = SessionState.Active;
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
    private static byte[]? GetSendData<T>(T data)
    {
        try
        {
            string name = Dispatch.GetSendMessageName(typeof(T));
            if (name.Equals(""))
                return null;
            Packet packet = new();
            packet.SetName(name);
            packet.SetContent(Serializer.Serialize(data));
            return Package.Pack(packet);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return null;
        }
    }
    public bool Send(byte[] data)
    {
        return _channel.ChannelState == RunState.On && _channel.Send(data);
    }
    public bool Send<T>(T data)
    {
        try
        {
            byte[]? result = GetSendData(data);
            return result != null && Send(result);
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
            byte[]? result = GetSendData(data);
            return result != null && SendAction != null && SendAction.Invoke(sessionId, result);
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
            byte[]? result = GetSendData(data);
            if (result == null)
                return;
            SendGroupAction?.Invoke(_sessionId, groupId, result);
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
            byte[]? result = GetSendData(data);
            if (result == null)
                return;
            SendBroadcastAction?.Invoke(_sessionId, result);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }

    private void Receive(byte[]? data)
    {
        if (data == null)
            return;

        List<Packet> packets = _dataParser.Parsing(data);
        if (packets is not { Count: > 0 })
            return;

        foreach (Packet? packet in packets)
            Dispatch.Dispense(this, packet);
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
