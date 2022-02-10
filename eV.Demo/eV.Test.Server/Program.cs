// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Reflection;
using eV.EasyLog;
using eV.EasyLog.Interface;
using eV.GameProfile;
using eV.GameProfile.Attributes;
using eV.Server;
using eV.Server.Base;
using eV.Server.Interface;
using eV.Server.Storage;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using eVServer = eV.Server.Server;
namespace eV.Test.Server
{
    internal static class Program
    {
        private static void Main()
        {
            // MongodbManager.Instance.Start();
            // TestData data = new()
            // {
            //     Id = new ObjectId("61fc97a93d3f0edba65442ca"),
            //     Name = "2222222222",
            //     UpdatedAt = DateTime.Now
            // };
            //
            // // collection?.InsertOne(data);
            // var filter = Builders<TestData>.Filter.Eq("_id", data.Id);
            // List<TestData> test = new();
            // test.Add(new TestData
            // {
            //     Name = "A"
            // });
            // test.Add(new TestData
            // {
            //     Name = "B"
            // });
            // test.Add(new TestData
            // {
            //     Name = "C"
            // });
            // // foreach (var t in test)
            // // {
            // //     t.Name = "s";
            // // }
            // // test.ForEach(d => d.Name = "SSSSSSSSSS");
            //
            // foreach (var t in test)
            // {
            //     Console.WriteLine(t.Name);
            // }
            //
            // // var documents = collection.Find(new BsonDocument()).ToList();
            // // foreach (var document in documents)
            // // {
            // //     Console.WriteLine(document.Name);
            // // }

            // GameProfile.Instance.Init("/Users/three.zhang/Projects/Unity/ws-config/json", true);
            // Console.WriteLine(GameProfile.Instance.Config["BattleMap"]["1:Id"]);
            // Console.WriteLine(GameProfile.Instance.Config["BattleMap"]["1:Name"]);

            // TestData test = new();
            // object s = "12313";
            // foreach (PropertyInfo propertyInfo in test.GetType().GetProperties())
            // {
            //     // Console.WriteLine();
            //     if (propertyInfo.Name == "Name")
            //         propertyInfo.SetValue(test, s);
            //     // propertyInfo.SetValue();
            // }
            // Console.WriteLine(test.Name);

            // Profile.OnLoad += delegate
            // {
            //     Profile.AssignmentConfigObject(Config.Instance);
            // };
            //
            // eVServer server = new();
            // server.Start();
            //
            //
            // // Console.WriteLine(Profile.Config.Count);
            // // Console.WriteLine(((BattleMap)Profile.Config["BattleMap"])["1"].Name);
            //
            // Console.WriteLine(Config.Instance.BattleMap!["1"].Name);
            Namelist.Instance.Name = "123";
            Console.WriteLine(Namelist.Instance.Name );
            Console.WriteLine(Namelist.Instance.Name );


            Console.ReadLine();
            // server.Stop();
        }
    }

    public class L : ILog{

        public void Debug(object message)
        {
            throw new NotImplementedException();
        }
        public void Debug(object message, Exception exception)
        {
            throw new NotImplementedException();
        }
        public void Info(object message)
        {
            Console.Write("11111111111111111");
        }
        public void Info(object message, Exception exception)
        {
            throw new NotImplementedException();
        }
        public void Warn(object message)
        {
            throw new NotImplementedException();
        }
        public void Warn(object message, Exception exception)
        {
            throw new NotImplementedException();
        }
        public void Error(object message)
        {
            throw new NotImplementedException();
        }
        public void Error(object message, Exception exception)
        {
            throw new NotImplementedException();
        }
        public void Fatal(object message)
        {
            throw new NotImplementedException();
        }
        public void Fatal(object message, Exception exception)
        {
            throw new NotImplementedException();
        }
    }

    public class TestData : ModelBase, IModel
    {
        public string Name { get; set; }
    }

    [ProfileStructure]
    public class BattleMap : Dictionary<string, Namelist>
    {

    }

    public class Config
    {
        public static Config Instance { get; } = new();

        private Config()
        {
        }
        public BattleMap? BattleMap { get; set; }
    }

    public class Namelist
    {
        public static Namelist Instance { get; } = new();
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
// [BsonIgnore]
// public const string Database = "eV";
//
// [BsonIgnore]
// public const string Collection = "TestData";
