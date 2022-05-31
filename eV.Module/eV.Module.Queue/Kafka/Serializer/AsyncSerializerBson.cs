// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using Confluent.Kafka;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
namespace eV.Module.Queue.Kafka.Serializer;

public class AsyncSerializerBson<T> : IAsyncSerializer<T>, IAsyncDeserializer<T>
{
    public Task<byte[]> SerializeAsync(T data, SerializationContext context)
    {
        return Task.FromResult(data.ToBson());
    }
    public Task<T> DeserializeAsync(ReadOnlyMemory<byte> data, bool isNull, SerializationContext context)
    {
        return Task.FromResult(BsonSerializer.Deserialize<T>(data.ToArray()));
    }
}
