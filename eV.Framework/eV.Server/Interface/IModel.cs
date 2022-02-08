// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;
namespace eV.Server.Interface
{
    public interface IModel
    {
        public ObjectId Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
