// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.Module.EasyLog;
using MongoDB.Driver;
namespace eV.Framework.Server.Storage;

public class MongodbManager
{

    private readonly Dictionary<string, MongoClient> _clients;
    private bool _isStart;

    private MongodbManager()
    {
        _isStart = false;
        _clients = new Dictionary<string, MongoClient>();
    }
    public static MongodbManager Instance
    {
        get;
    } = new();

    public void Start()
    {
        if (_isStart)
            return;
        _isStart = true;

        if (Configure.Instance.StorageOptions.Mongodb == null)
        {
            Logger.Warn("Mongodb config is null");
            return;
        }
        foreach ((string? dbName, string? connString) in Configure.Instance.StorageOptions.Mongodb)
            try
            {
                MongoClient client = new(connString);
                _clients.Add(dbName, client);
                Logger.Info($"Mongodb [{dbName}] connected success");
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
    }

    public MongoClient? GetClient(string name)
    {
        _clients.TryGetValue(name, out MongoClient? client);
        return client;
    }
    public IMongoDatabase? GetDatabase(string database)
    {
       return _clients.TryGetValue(database, out MongoClient? client) ? client.GetDatabase(database) : null;
    }
    public IMongoCollection<T>? GetCollection<T>(string database, string collection)
    {
        return _clients.TryGetValue(database, out MongoClient? client) ? client.GetDatabase(database).GetCollection<T>(collection) : null;
    }
}
