// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using Confluent.Kafka;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
namespace eV.Module.Queue.Kafka.Serializer;

public class SerializeBson<T> : ISerializer<T>, IDeserializer<T>
{
    public byte[] Serialize(T data, SerializationContext context)
    {
        return data.ToBson();
    }
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
       return BsonSerializer.Deserialize<T>(data.ToArray());
    }
}
