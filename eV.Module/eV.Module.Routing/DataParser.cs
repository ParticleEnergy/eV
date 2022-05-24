// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.Module.Routing.Interface;
namespace eV.Module.Routing;

public class DataParser
{
    private IPacket? _currentPacket;
    private byte[] _lastReceiveBuffer = Array.Empty<byte>();

    public void Reset()
    {
        _lastReceiveBuffer = Array.Empty<byte>();
        _currentPacket = null;
    }

    public List<Packet> Parsing(byte[] data)
    {
        List<Packet> result = new();
        int current = 0;
        data = GetReceiveBuffer(data);
        while (true)
        {
            if (!CheckReceiveBufferLength(data, current, Package.HandLength))
                break;

            if (_currentPacket == null)
            {
                byte[] hand = data.Skip(current).Take(Package.HandLength).ToArray();
                current += Package.HandLength;
                _currentPacket = Package.Unpack(hand);
            }

            if (!CheckReceiveBufferLength(data, current, _currentPacket!.GetNameLength() + _currentPacket!.GetContentLength()))
                break;

            if (_currentPacket.GetNameLength() > 0)
            {
                Packet packet = new();
                packet.SetName(Encoder.GetEncoding().GetString(data.Skip(current).Take(_currentPacket.GetNameLength()).ToArray()));
                current += _currentPacket.GetNameLength();

                if (_currentPacket.GetContentLength() > 0)
                {
                    packet.SetContent(data.Skip(current).Take(_currentPacket.GetContentLength()).ToArray());
                    current += _currentPacket.GetContentLength();
                }
                result.Add(packet);
            }
            _currentPacket = null;
        }
        return result;
    }

    private byte[] GetReceiveBuffer(byte[] data)
    {
        if (_lastReceiveBuffer.Length <= 0)
            return data;
        MemoryStream memoryStream = new();
        memoryStream.Write(_lastReceiveBuffer, 0, _lastReceiveBuffer.Length);
        memoryStream.Write(data, 0, data.Length);
        return memoryStream.ToArray();
    }

    private bool CheckReceiveBufferLength(IReadOnlyCollection<byte> data, int current, int flagLength)
    {
        int residueLength = data.Count - current;
        if (residueLength >= flagLength)
            return true;

        _lastReceiveBuffer = new byte[residueLength];
        _lastReceiveBuffer = data.Skip(current).Take(residueLength).ToArray();
        return false;
    }
}
