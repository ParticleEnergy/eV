// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace eV.Module.Routing;

public static class Serializer
{
    public static byte[] Serialize<T>(T data)
    {
        return GZip.Compress(data.ToBson());
    }

    public static object Deserialize(byte[] data, Type type)
    {
        return BsonSerializer.Deserialize(GZip.Decompress(data), type);
    }
}
