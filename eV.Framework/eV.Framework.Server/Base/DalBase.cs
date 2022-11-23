// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Module.Storage.Mongo;
using eV.Module.Storage.Mongo.Interface;
using MongoDB.Driver;

namespace eV.Framework.Server.Base;

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

    public virtual void Insert(T data)
    {
        MongodbHelper.Insert(Database, Collection, data);
    }

    public virtual void Insert(List<T> data)
    {
        MongodbHelper.Insert(Database, Collection, data);
    }

    public virtual async Task InsertAsync(T data)
    {
        await MongodbHelper.InsertAsync(Database, Collection, data);
    }

    public virtual async Task InsertAsync(List<T> data)
    {
        await MongodbHelper.InsertAsync(Database, Collection, data);
    }

    #endregion

    #region Delete

    public virtual DeleteResult? Delete(FilterDefinition<T> filter, bool one = true)
    {
        return MongodbHelper.Delete(Database, Collection, filter, one);
    }

    public virtual async Task<DeleteResult?> DeleteAsync(FilterDefinition<T> filter, bool one = true)
    {
        return await MongodbHelper.DeleteAsync(Database, Collection, filter, one);
    }

    #endregion

    #region Update

    public virtual UpdateResult? Update(FilterDefinition<T> filter, UpdateDefinition<T> update, bool one = true, bool isUpsert = false)
    {
        return MongodbHelper.Update(Database, Collection, filter, update, one, isUpsert);
    }

    public virtual async Task<UpdateResult?> UpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, bool one = true, bool isUpsert = false)
    {
        return await MongodbHelper.UpdateAsync(Database, Collection, filter, update, one, isUpsert);
    }

    #endregion

    #region Select

    public virtual IAsyncCursor<T>? Select(FilterDefinition<T> filter, int offset, int limit)
    {
        return MongodbHelper.Select(Database, Collection, filter, offset, limit);
    }

    public virtual async Task<IAsyncCursor<T>?> SelectAsync(FilterDefinition<T> filter, int offset, int limit)
    {
        return await MongodbHelper.SelectAsync(Database, Collection, filter, offset, limit);
    }

    #endregion

    #region Replace

    public virtual ReplaceOneResult? Replace(FilterDefinition<T> filter, T data, bool isUpsert = false)
    {
        return MongodbHelper.Replace(Database, Collection, filter, data, isUpsert);
    }

    public virtual async Task<ReplaceOneResult?> ReplaceAsync(FilterDefinition<T> filter, T data, bool isUpsert = false)
    {
        return await MongodbHelper.ReplaceAsync(Database, Collection, filter, data, isUpsert);
    }

    #endregion
}
