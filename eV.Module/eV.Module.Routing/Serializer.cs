// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
namespace eV.Module.Routing;

public static class Serializer
{
    private static bool _isCompress;
    public static void EnableCompress()
    {
        _isCompress = true;
    }

    public static void DisableCompress()
    {
        _isCompress = false;
    }

    public static byte[] Serialize<T>(T data)
    {
        return _isCompress ? GZip.Compress(data.ToBson()) : data.ToBson();
    }
    public static object Deserialize(byte[] data, Type type)
    {
        return BsonSerializer.Deserialize(_isCompress ? GZip.Decompress(data) : data, type);
    }
}
