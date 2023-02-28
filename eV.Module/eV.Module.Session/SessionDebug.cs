// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using System.Text.Json;
using eV.Module.EasyLog;

namespace eV.Module.Session;

public static class SessionDebug
{
    private static bool _isDebug;

    public static void EnableDebug()
    {
        _isDebug = true;
    }

    public static void DebugReceive(string? sessionId, string name, object content)
    {
        Logger.Debug(_isDebug
            ? $"ReceiveMessage [{name}] [{sessionId}] {JsonSerializer.Serialize(content)}"
            : $"ReceiveMessage [{name}] [{sessionId}]");
    }

    public static void DebugSend<T>(string? sessionId, string name, T content)
    {
        if (!_isDebug) return;

        Logger.Debug($"Send [{name}] [{sessionId}] {JsonSerializer.Serialize(content)}");
    }

    public static void DebugSend<T>(string? sessionId, string toSessionId, string name, T content)
    {
        if (!_isDebug) return;

        Logger.Debug($"SendBySessionId [{name}] [{sessionId}] [{toSessionId}] {JsonSerializer.Serialize(content)}");
    }

    public static void DebugSendBroadcast<T>(string? sessionId, string name, T content)
    {
        if (!_isDebug) return;

        Logger.Debug($"SendBroadcast [{name}] [{sessionId}] {JsonSerializer.Serialize(content)}");
    }
}
