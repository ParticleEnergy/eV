// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.IO.Compression;

namespace eV.Module.Routing;

public static class GZip
{
    public static byte[] Compress(byte[] data)
    {
        MemoryStream outStream = new();
        GZipStream gZipStream = new(outStream, CompressionMode.Compress, true);
        gZipStream.Write(data, 0, data.Length);
        gZipStream.Close();
        return outStream.ToArray();
    }

    public static byte[] Decompress(byte[] data)
    {
        MemoryStream inStream = new(data);
        MemoryStream outStream = new();
        GZipStream gZipStream = new(inStream, CompressionMode.Decompress);
        gZipStream.CopyTo(outStream);
        gZipStream.Close();
        return outStream.ToArray();
    }
}
