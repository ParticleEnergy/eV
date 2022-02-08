// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using log4net;
using MongoDB.Driver;
namespace eV.Server.Storage
{
    public class MongodbManager
    {
        public static MongodbManager Instance
        {
            get;
        } = new();

        private readonly Dictionary<string, MongoClient> _clients;
        private readonly ILog _logger = LogManager.GetLogger(DefaultSetting.LoggerName);
        private bool _isStart;

        private MongodbManager()
        {
            _isStart = false;
            _clients = new Dictionary<string, MongoClient>();
        }

        public void Start()
        {
            if (_isStart)
                return;
            _isStart = true;

            if (Configure.Instance.StorageOptions.Mongodb == null)
            {
                _logger.Warn("Mongodb config is null");
                return;
            }
            foreach (var (dbName, connString) in Configure.Instance.StorageOptions.Mongodb)
            {
                try
                {
                    MongoClient client = new(connString);
                    _clients.Add(dbName, client);
                    _logger.Info($"Mongodb [{dbName}] connected success");
                }
                catch (Exception e)
                {
                    _logger.Error(e.Message, e);
                }
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
}
