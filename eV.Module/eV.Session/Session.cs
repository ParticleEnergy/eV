// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using eV.Network.Core;
using eV.Routing;
using eV.Routing.Interface;
using log4net;
namespace eV.Session
{
    public class Session : ISession
    {
        #region Event
        public event SessionEvent? OnActivate;
        public event SessionEvent? OnRelease;
        #endregion

        #region Action
        public Func<string, byte[], bool>? SendAction;
        public Action<string, byte[]>? SendGroupAction;
        public Action<byte[]>? SendBroadcastAction;
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
            get
            {
                if (_channel.LastSendDateTime == null && _channel.LastReceiveDateTime == null)
                {
                    return _channel.ConnectedDateTime;
                }
                if (_channel.LastSendDateTime != null && _channel.LastReceiveDateTime != null)
                {
                    return _channel.LastSendDateTime >= _channel.LastReceiveDateTime ? _channel.LastSendDateTime : _channel.LastReceiveDateTime;
                }
                return _channel.LastSendDateTime ?? _channel.LastReceiveDateTime;
            }
            set {}
        }
        #endregion

        private readonly Channel _channel;
        private readonly DataParser _dataParser = new();
        private readonly ILog _logger = LogManager.GetLogger(DefaultSetting.LoggerName);

        public Session(Channel channel)
        {
            SessionState = SessionState.Free;
            SessionData = new Hashtable();
            // Channel
            _channel = channel;
            _channel.Receive = Receive;
            _channel.CloseCompleted += ChannelClose;
        }

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
            {
                _channel.Close();
            }
            Release();
        }
        private void Release()
        {
            OnRelease?.Invoke(this);
            foreach (var g in _group)
            {
                LeaveGroup(g.Value);
            }
            _sessionId = null;
            _group.Clear();
            SessionData.Clear();
            SessionState = SessionState.Free;
        }
        #endregion

        #region Channel
        private void ChannelClose(Channel channel)
        {
            Release();
        }
        #endregion


        #region IO
        private byte[]? GetSendData<T>(T data)
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
                _logger.Error(e.Message, e);
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
                _logger.Error(e.Message, e);
                return false;
            }
        }
        public bool Send<T>(string sessionId, T data)
        {
            try
            {
                byte[]? result = GetSendData(data);
                return result != null && SendAction != null && SendAction.Invoke(sessionId, result);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
                return false;
            }
        }
        public void SendGroup<T>(string groupId, T data)
        {
            try
            {
                byte[]? result = GetSendData(data);
                if (result == null)
                    return;
                SendGroupAction?.Invoke(groupId, result);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
            }
        }
        public void SendBroadcast<T>(T data)
        {
            try
            {
                byte[]? result = GetSendData(data);
                if (result == null)
                    return;
                SendBroadcastAction?.Invoke(result);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
            }
        }

        private void Receive(byte[]? data)
        {
            if (data == null)
                return;

            List<Packet> packets = _dataParser.Parsing(data);
            if (packets is not { Count: > 0 })
                return;

            foreach (var packet in packets)
            {
                Dispatch.Dispense(this, packet);
            }
        }
        #endregion

        #region Group
        public bool JoinGroup(string groupName)
        {
            if (JoinGroupAction == null || _sessionId is null or "")
                return false;
            if (!JoinGroupAction.Invoke(groupName, _sessionId))
                return false;
            _group[groupName] = _sessionId;
            return true;
        }
        public bool LeaveGroup(string groupName)
        {
            if (LeaveGroupAction == null || _sessionId is null or "")
                return false;
            if (!LeaveGroupAction.Invoke(groupName, _sessionId))
                return false;
            _group.Remove(groupName);
            return true;
        }
        #endregion
    }
}
