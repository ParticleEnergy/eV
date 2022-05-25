// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.Module.EasyLog;
using eV.Module.Storage.Mongo.Interface;
using MongoDB.Driver;
namespace eV.Module.Storage.Mongo;

public static class MongodbHelper
{
    #region Resource
    public static IMongoClient? GetMongoClient(string name)
    {
        MongoClient? client = MongodbManager.Instance.GetClient(name);
        if (client == null)
            Logger.Error($"Mongo client database [{name}] not found");
        return client;
    }
    public static IMongoDatabase? GetDatabase(string database)
    {
        IMongoDatabase? db = GetMongoClient(database)?.GetDatabase(database);
        if (db == null)
            Logger.Error($"Mongo database [{database}] not found");
        return db;
    }
    public static IMongoCollection<T>? GetCollection<T>(string database, string collection) where T : IModel
    {
        IMongoCollection<T>? c = GetDatabase(database)?.GetCollection<T>(collection);
        if (c == null)
            Logger.Error($"Mongo database [{database}] collection [{collection}] not found");
        return c;
    }
    #endregion

    #region Insert
    public static void Insert<T>(string database, string collection, T data) where T : IModel
    {
        try
        {
            data.CreatedAt = DateTime.Now;
            GetCollection<T>(database, collection)?.InsertOne(data);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }
    public static void Insert<T>(string database, string collection, List<T> data) where T : class, IModel
    {
        try
        {
            data.ForEach(d => d.CreatedAt = DateTime.Now);
            GetCollection<T>(database, collection)?.InsertMany(data);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
        }
    }
    public static Task? InsertAsync<T>(string database, string collection, T data) where T : IModel
    {
        try
        {
            data.CreatedAt = DateTime.Now;
            return GetCollection<T>(database, collection)?.InsertOneAsync(data);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return null;
        }
    }
    public static Task? InsertAsync<T>(string database, string collection, List<T> data) where T : class, IModel
    {
        try
        {
            data.ForEach(d => d.CreatedAt = DateTime.Now);
            return GetCollection<T>(database, collection)?.InsertManyAsync(data);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return null;
        }
    }
    #endregion

    #region Delete
    public static DeleteResult? Delete<T>(string database, string collection, FilterDefinition<T> filter, bool one = true) where T : IModel
    {
        try
        {
            return one ? GetCollection<T>(database, collection)?.DeleteOne(filter) : GetCollection<T>(database, collection)?.DeleteMany(filter);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return null;
        }
    }
    public static Task<DeleteResult>? DeleteAsync<T>(string database, string collection, FilterDefinition<T> filter, bool one = true) where T : IModel
    {
        try
        {
            return one ? GetCollection<T>(database, collection)?.DeleteOneAsync(filter) : GetCollection<T>(database, collection)?.DeleteManyAsync(filter);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return null;
        }
    }
    #endregion

    #region Update
    public static UpdateResult? Update<T>(string database, string collection, FilterDefinition<T> filter, UpdateDefinition<T> update, bool one = true, bool isUpsert = false) where T : IModel
    {
        try
        {
            update.Set("UpdatedAt", DateTime.Now);
            return one ? GetCollection<T>(database, collection)?.UpdateOne(filter, update, new UpdateOptions
            {
                IsUpsert = isUpsert
            }) : GetCollection<T>(database, collection)?.UpdateMany(filter, update, new UpdateOptions
            {
                IsUpsert = isUpsert
            });
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return null;
        }
    }
    public static Task<UpdateResult>? UpdateAsync<T>(string database, string collection, FilterDefinition<T> filter, UpdateDefinition<T> update, bool one = true, bool isUpsert = false) where T : IModel
    {
        try
        {
            update.Set("UpdatedAt", DateTime.Now);
            return one ? GetCollection<T>(database, collection)?.UpdateOneAsync(filter, update, new UpdateOptions
            {
                IsUpsert = isUpsert
            }) : GetCollection<T>(database, collection)?.UpdateManyAsync(filter, update, new UpdateOptions
            {
                IsUpsert = isUpsert
            });
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return null;
        }
    }
    #endregion

    #region Select
    public static IAsyncCursor<T>? Select<T>(string database, string collection, FilterDefinition<T> filter, int offset, int limit) where T : IModel
    {
        try
        {
            return GetCollection<T>(database, collection)?.FindSync(filter, new FindOptions<T>
            {
                Skip = offset, Limit = limit
            });
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return null;
        }
    }
    public static Task<IAsyncCursor<T>>? SelectAsync<T>(string database, string collection, FilterDefinition<T> filter, int offset, int limit) where T : IModel
    {
        try
        {
            return GetCollection<T>(database, collection)?.FindAsync(filter, new FindOptions<T>
            {
                Skip = offset, Limit = limit
            });
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return null;
        }
    }
    #endregion

    #region Replace
    public static ReplaceOneResult? Replace<T>(string database, string collection, FilterDefinition<T> filter, T data, bool isUpsert = false) where T : IModel
    {
        try
        {
            data.UpdatedAt = DateTime.Now;
            return GetCollection<T>(database, collection)?.ReplaceOne(filter, data, new ReplaceOptions
            {
                IsUpsert = isUpsert
            });
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return null;
        }
    }
    public static Task<ReplaceOneResult>? ReplaceAsync<T>(string database, string collection, FilterDefinition<T> filter, T data, bool isUpsert = false) where T : IModel
    {
        try
        {
            data.UpdatedAt = DateTime.Now;
            return GetCollection<T>(database, collection)?.ReplaceOneAsync(filter, data, new ReplaceOptions
            {
                IsUpsert = isUpsert
            });
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e);
            return null;
        }
    }
    #endregion
}
