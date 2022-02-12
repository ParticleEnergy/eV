// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.


using eV.Server.Interface;
using eV.Server.Storage;
using MongoDB.Driver;
namespace eV.Server.Base;

public abstract class DalBase<T> where T : class, IModel
{
    public abstract string Database { get; }
    public abstract string Collection { get; }

    public IMongoClient GetMongoClient()
    {
        return MongodbHelper.GetMongoClient(Database)!;
    }
    public IMongoDatabase GetDatabase()
    {
        return MongodbHelper.GetDatabase(Database)!;
    }
    public IMongoCollection<T> GetCollection()
    {
        return MongodbHelper.GetCollection<T>(Database, Collection)!;
    }

    #region Insert
    public void Insert(T data)
    {
        MongodbHelper.Insert(Database, Collection, data);
    }
    public void Insert(List<T> data)
    {
        MongodbHelper.Insert(Database, Collection, data);
    }
    public Task? InsertAsync(T data)
    {
        return MongodbHelper.InsertAsync(Database, Collection, data);
    }
    public Task? InsertAsync(List<T> data)
    {
        return MongodbHelper.InsertAsync(Database, Collection, data);
    }
    #endregion

    #region Delete
    public DeleteResult? Delete(FilterDefinition<T> filter, bool one = true)
    {
        return MongodbHelper.Delete(Database, Collection, filter, one);
    }
    public Task<DeleteResult>? DeleteAsync(FilterDefinition<T> filter, bool one = true)
    {
        return MongodbHelper.DeleteAsync(Database, Collection, filter, one);
    }
    #endregion

    #region Update
    public UpdateResult? Update(FilterDefinition<T> filter, UpdateDefinition<T> update, bool one = true, bool isUpsert = false)
    {
        return MongodbHelper.Update(Database, Collection, filter, update, one, isUpsert);
    }
    public Task<UpdateResult>? UpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, bool one = true, bool isUpsert = false)
    {
        return MongodbHelper.UpdateAsync(Database, Collection, filter, update, one, isUpsert);
    }
    #endregion

    #region Select
    public IAsyncCursor<T>? Select(FilterDefinition<T> filter, int offset, int limit)
    {
        return MongodbHelper.Select(Database, Collection, filter, offset, limit);
    }
    public Task<IAsyncCursor<T>>? SelectAsync(FilterDefinition<T> filter, int offset, int limit)
    {
        return MongodbHelper.SelectAsync(Database, Collection, filter, offset, limit);
    }
    #endregion

    #region Replace
    public ReplaceOneResult? Replace(FilterDefinition<T> filter, T data, bool isUpsert = false)
    {
        return MongodbHelper.Replace(Database, Collection, filter, data, isUpsert);
    }
    public Task<ReplaceOneResult>? ReplaceAsync(FilterDefinition<T> filter, T data, bool isUpsert = false)
    {
        return MongodbHelper.ReplaceAsync(Database, Collection, filter, data, isUpsert);
    }
    #endregion
}
