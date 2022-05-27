// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using MongoDB.Bson;
namespace eV.Module.Storage.Mongo.Interface;

public interface IModel
{
    public ObjectId Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
