// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Module.EasyLog;
using MongoDB.Bson;

namespace eV.Module.Session;

public static class SessionDebug
{
    private static bool _isDebug;

    public static void EnableDebug()
    {
        _isDebug = true;
    }

    public static void DisableDebug()
    {
        _isDebug = false;
    }

    public static void DebugReceive(string? sessionId, string name, object content)
    {
        if (_isDebug)
        {
            Logger.Debug($"ReceiveMessage - SessionId {sessionId} Message {name} Content {content.ToJson()}");
        }
        else
        {
            Logger.Info($"ReceiveMessage - SessionId {sessionId} Message {name}");
        }
    }

    public static void DebugSend<T>(string? sessionId, string name, T content)
    {
        if (!_isDebug) return;

        Logger.Debug($"Send - SessionId {sessionId} Message {name} Content {content.ToJson()}");
    }

    public static void DebugSend<T>(string? sessionId, string toSessionId, string name, T content)
    {
        if (!_isDebug) return;

        Logger.Debug($"SendToSessionId - SessionId {sessionId} to SessionId {toSessionId} Message {name} Content {content.ToJson()}");
    }

    public static void DebugSendGroup<T>(string? sessionId, string groupId, string name, T content)
    {
        if (!_isDebug) return;

        Logger.Debug($"DebugSendGroup - SessionId {sessionId} GroupId {groupId} Message {name} Content {content.ToJson()}");
    }

    public static void DebugSendBroadcast<T>(string? sessionId, string name, T content)
    {
        if (!_isDebug) return;

        Logger.Debug($"SendBroadcast - SessionId {sessionId} Message {name} Content {content.ToJson()}");
    }
}

