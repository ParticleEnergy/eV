// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


namespace eV.Module.Cluster.Communication;

public static class CommunicationStream
{
    public static string GetSendStream(int consumer)
    {
        return $"Send:{consumer}";
    }

    public static string GetSendBroadcastStream(int consumer)
    {
        return $"SendBroadcast:{consumer}";
    }

    public static string GetSessionIdKey()
    {
        return "sessionId";
    }

    public static string GetBodyKey()
    {
        return "body";
    }
}
