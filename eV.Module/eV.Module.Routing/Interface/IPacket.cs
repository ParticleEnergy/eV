// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Module.Routing.Interface;

public interface IPacket
{
    public void SetName(string name);
    public void SetName(byte[] nameBytes);
    public string GetName();
    public byte[] GetNameBytes();
    public void SetNameLength(int length);
    public int GetNameLength();
    public void SetContent(byte[] content);
    public byte[] GetContent();
    public void SetContentLength(int length);
    public int GetContentLength();
}
