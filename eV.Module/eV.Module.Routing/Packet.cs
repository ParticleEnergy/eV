// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Routing.Interface;

namespace eV.Module.Routing;

public class Packet : IPacket
{
    private byte[]? _content;
    private int? _contentLength;
    private string? _name;
    private byte[]? _nameBytes;
    private int? _nameLength;

    public void SetName(string name)
    {
        _name = name;
        _nameBytes = Encoder.GetEncoding().GetBytes(name);
    }

    public void SetName(byte[] nameBytes)
    {
        _nameBytes = nameBytes;
        _name = Encoder.GetEncoding().GetString(nameBytes);
    }

    public string GetName()
    {
        return _name ?? "";
    }

    public byte[] GetNameBytes()
    {
        return _nameBytes ?? Array.Empty<byte>();
    }

    public void SetNameLength(int length)
    {
        _nameLength = length;
    }

    public int GetNameLength()
    {
        return _nameLength ?? _nameBytes?.Length ?? 0;
    }

    public void SetContent(byte[] content)
    {
        _content = content;
    }

    public byte[] GetContent()
    {
        return _content ?? Array.Empty<byte>();
    }

    public void SetContentLength(int length)
    {
        _contentLength = length;
    }

    public int GetContentLength()
    {
        return _contentLength ?? _content?.Length ?? 0;
    }
}
