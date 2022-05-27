// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Routing.Interface;
namespace eV.Module.Routing;

public static class Package
{
    public const int HandLength = 8;

    public static byte[] Pack(IPacket packet)
    {
        int total = HandLength + packet.GetNameLength() + packet.GetContentLength();
        MemoryStream memoryStream = new(total);
        memoryStream.Write(BitConverter.GetBytes(packet.GetNameLength()), 0, 4);
        memoryStream.Write(BitConverter.GetBytes(packet.GetContentLength()), 0, 4);
        memoryStream.Write(packet.GetNameBytes(), 0, packet.GetNameBytes().Length);
        if (packet.GetContentLength() > 0)
            memoryStream.Write(packet.GetContent(), 0, packet.GetContent().Length);

        return memoryStream.ToArray();
    }

    public static IPacket Unpack(byte[] data)
    {
        Packet packet = new();
        packet.SetNameLength(BitConverter.ToInt32(data.Skip(0).Take(4).ToArray(), 0));
        packet.SetContentLength(BitConverter.ToInt32(data.Skip(4).Take(4).ToArray(), 0));
        return packet;
    }
}
