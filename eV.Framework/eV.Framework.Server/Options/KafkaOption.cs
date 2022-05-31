// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Framework.Server.Options;

public class KafkaOption
{
    public string Address { get; set; } = string.Empty;
    public string SaslMechanism { get; set; } = string.Empty;
    public string SecurityProtocol { get; set; } = string.Empty;
    public string SaslUsername { get; set; } = string.Empty;
    public string SaslPassword { get; set; } = string.Empty;

    public int HeartbeatIntervalMs { get; set; } = 0;
    public int SessionTimeoutMs { get; set; } = 0;

    public int SocketTimeoutMs { get; set; } = 0;
    public int SocketReceiveBufferBytes { get; set; } = 0;
    public int SocketSendBufferBytes { get; set; } = 0;
}
