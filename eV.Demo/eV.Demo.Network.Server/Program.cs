// See https://aka.ms/new-console-template for more information

using System;
using System.Text;
using eV.Network.Core;
using eV.Network.Server;
using log4net;
namespace eV.Demo.Network.Server
{
    internal static class Program
    {
        private static void Main()
        {
            var log = LogManager.GetLogger("test");
            log.Info("server");
            ServerSetting setting = new();
            setting.Address = "10.0.0.33";
            eV.Network.Server.Server server = new(setting);
            server.Start();
            server.AcceptConnect += delegate(Channel channel)
            {
                channel.Receive = delegate(byte[]? data)
                {
                    if (data is not null)
                    {
                        Console.WriteLine($"{channel.ChannelId} data length: {data.Length}");
                        Console.WriteLine($"{channel.ChannelId} {Encoding.Default.GetString(data)}");
                    }
                };
            };
            Console.ReadLine();
            server.Stop();
            Console.ReadLine();
        }
    }
}
