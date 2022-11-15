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
        Logger.Debug(_isDebug
            ? $"ReceiveMessage [{name}] [{sessionId}] {content.ToJson()}"
            : $"ReceiveMessage [{name}] [{sessionId}]");
    }

    public static void DebugSend<T>(string? sessionId, string name, T content)
    {
        if (!_isDebug) return;

        Logger.Debug($"Send [{name}] [{sessionId}] {content.ToJson()}");
    }

    public static void DebugSend<T>(string? sessionId, string toSessionId, string name, T content)
    {
        if (!_isDebug) return;

        Logger.Debug($"SendBySessionId [{name}] [{sessionId}] [{toSessionId}] {content.ToJson()}");
    }

    public static void DebugSendGroup<T>(string? sessionId, string groupId, string name, T content)
    {
        if (!_isDebug) return;

        Logger.Debug($"SendGroup [{name}] [{sessionId}] [{groupId}] {content.ToJson()}");
    }

    public static void DebugSendBroadcast<T>(string? sessionId, string name, T content)
    {
        if (!_isDebug) return;

        Logger.Debug($"SendBroadcast [{name}] [{sessionId}] {content.ToJson()}");
    }
}
