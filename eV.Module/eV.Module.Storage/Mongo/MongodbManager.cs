// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Module.EasyLog;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

namespace eV.Module.Storage.Mongo;

public class MongodbManager
{
    private readonly Dictionary<string, MongoClient> _clients;
    private bool _isStart;

    private MongodbManager()
    {
        _isStart = false;
        _clients = new Dictionary<string, MongoClient>();
    }

    public static MongodbManager Instance { get; } = new();

    public void Start(Dictionary<string, string> config)
    {
        if (_isStart)
            return;
        _isStart = true;

        foreach ((string dbName, string connString) in config)
            try
            {
                var mongoConnectionUrl = new MongoUrl(connString);
                var mongoClientSettings = MongoClientSettings.FromUrl(mongoConnectionUrl);
                if (Logger.IsDebug())
                {
                    mongoClientSettings.ClusterConfigurator = clusterBuilder =>
                    {
                        clusterBuilder.Subscribe<CommandStartedEvent>(commandStartedEvent =>
                        {
                            Logger.Debug(
                                $"Mongodb [{commandStartedEvent.CommandName}] {commandStartedEvent.Command.ToJson()}");
                        });
                    };
                }

                MongoClient client = new(mongoClientSettings);
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
        return _clients.TryGetValue(database, out MongoClient? client)
            ? client.GetDatabase(database).GetCollection<T>(collection)
            : null;
    }
}
