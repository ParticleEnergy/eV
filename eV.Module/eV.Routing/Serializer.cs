// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
namespace eV.Routing
{
    public static class Serializer
    {
        public static byte[] Serialize<T>(T data)
        {
            return data.ToBson();
        }
        public static object Deserialize(byte[] data, Type type)
        {
            return BsonSerializer.Deserialize(data, type);
        }
    }
}
