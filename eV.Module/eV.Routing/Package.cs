// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using eV.Routing.Interface;
namespace eV.Routing
{
    public static class Package
    {
        public const int HandLength = 8;

        public static byte[] Pack(IPacket packet)
        {
            int total = HandLength + packet.GetNameLength() + packet.GetContentLength();
            MemoryStream memoryStream = new(total);
            memoryStream.Write(BitConverter.GetBytes(packet.GetNameLength()));
            memoryStream.Write(BitConverter.GetBytes(packet.GetContentLength()));
            memoryStream.Write(packet.GetNameBytes());
            if (packet.GetContentLength() > 0)
                memoryStream.Write(packet.GetContent());

            return memoryStream.ToArray();
        }

        public static IPacket Unpack(byte[] data)
        {
            Packet packet = new();
            packet.SetNameLength(BitConverter.ToInt32(data.Skip(0).Take(4).ToArray()));
            packet.SetContentLength(BitConverter.ToInt32(data.Skip(4).Take(4).ToArray()));
            return packet;
        }
    }
}
