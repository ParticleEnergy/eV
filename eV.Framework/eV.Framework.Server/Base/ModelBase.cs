// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.Module.Storage.Mongo.Interface;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace eV.Framework.Server.Base;

public class ModelBase : IModel
{
    [BsonId]
    [BsonElement("_id")]
    public ObjectId Id { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime CreatedAt { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime UpdatedAt { get; set; }
}
