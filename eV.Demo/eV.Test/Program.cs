// See https://aka.ms/new-console-template for more information

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using eV.Routing;
using eV.Routing.Interface;
using eV.Session;
using eV.Test.Handlers;

namespace eV.Test
{

    internal static class Program
    {
        private static void Main()
        {
            // SessionGroup sessionGroup = new();
            // sessionGroup.CreateGroup("test");
            //
            // sessionGroup.JoinGroup("test", "abc");
            // foreach (var value in sessionGroup.GetGroup("test"))
            // {
            //     Console.WriteLine(value.Value);
            // }
            // sessionGroup.LeaveGroup("test", "abc");
            // sessionGroup.JoinGroup("test", "11111");
            //
            // foreach (var value in sessionGroup.GetGroup("test"))
            // {
            //     Console.WriteLine(value.Value);
            // }

            IndexParam indexParam = new()
            {
                Name = "indexParam"
            };
            IndexParam indexParam1 = new()
            {
                Name = "indexParam1"
            };
            Packet packet = new();
            packet.SetName("IndexParam");
            packet.SetContent(Serializer.Serialize(indexParam));
            byte[] data = Package.Pack(packet);
            Packet packet1 = new();
            packet1.SetName("IndexParam1");
            packet1.SetContent(Serializer.Serialize(indexParam1));
            byte[] data1 = Package.Pack(packet1);

            MemoryStream memoryStream = new();
            memoryStream.Write(data);
            memoryStream.Write(data1);

            // Console.WriteLine(data.Length);
            // var msg = (Message)package.Unpack(data);
            //
            // Console.WriteLine(msg.GetHandlerLength());
            // Console.WriteLine(msg.GetDataLength());
            //
            // Console.WriteLine(package.GetEncoding().GetString(data.Skip(12).Take(5).ToArray()));

            DataParser dataParser = new();
            List<Packet> result = dataParser.Parsing(memoryStream.ToArray());
            // Console.WriteLine(result.Count);

            Dispatch.Register("eV.Test");
            Session session = new();
            foreach (var rst in result)
            {
                Console.WriteLine(rst.GetNameLength());
                Dispatch.Dispense(session, rst);

                // Console.WriteLine(rst.GetName());
                // Console.WriteLine(((IndexParam)Serializer.Deserialize(rst.GetContent(), typeof(IndexParam))).Name);
            }

            Console.ReadLine();
        }
    }


    public class Session : ISession
    {

        public string? SessionId
        {
            get;
            set;
        }
        public Hashtable SessionData
        {
            get;
        }
        public Dictionary<string, string> Group
        {
            get;
            set;
        }
        public DateTime? ConnectedDateTime
        {
            get;
            set;
        }
        public DateTime? LastActiveDateTime
        {
            get;
            set;
        }
        public bool Send(byte[] data)
        {
            throw new NotImplementedException();
        }
        public bool Send<T>(T data)
        {
            throw new NotImplementedException();
        }
        public bool Send<T>(string sessionId, T data)
        {
            throw new NotImplementedException();
        }
        public void SendGroup<T>(string groupId, T data)
        {
            throw new NotImplementedException();
        }
        public void SendBroadcast<T>(T data)
        {
            throw new NotImplementedException();
        }
        public bool JoinGroup(string groupName)
        {
            throw new NotImplementedException();
        }
        public bool LeaveGroup(string groupName)
        {
            throw new NotImplementedException();
        }
        public void Activate(string sessionId)
        {
            throw new NotImplementedException();
        }
        public void Shutdown()
        {
            throw new NotImplementedException();
        }
    }


}
