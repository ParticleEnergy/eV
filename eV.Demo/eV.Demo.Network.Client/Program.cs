// See https://aka.ms/new-console-template for more information

using System;
using System.Text;
using System.Threading;
using eV.Network.Client;
using eV.Network.Core;
using log4net;
namespace eV.Demo.Network.Client
{
    internal static class Program
    {
        private static void Main()
        {
            LogManager.GetLogger("test").Info("info");
            ClientSetting setting = new();
            // setting.Address = "10.0.0.33";
            eV.Network.Client.Client client = new(setting);
            client.DisconnectCompleted += delegate(Channel channel)
            {
                Console.WriteLine($"{channel.ChannelId}   cleint Disconnect .............................");
                // client.Connect();
            };
            client.ConnectCompleted += delegate(Channel channel)
            {
                channel.Receive = delegate(byte[]? bytes)
                {
                    if (bytes != null)
                        Console.WriteLine(Encoding.UTF8.GetString(bytes));
                };
                // for (int i = 0; i < 100; i++)
                // {
                //     try
                //     {
                //         // Thread.Sleep(2000); //等待1秒钟
                //         string sendMessage = "ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ";
                //         Console.WriteLine(channel.Send(Encoding.UTF8.GetBytes(sendMessage)));
                //         Console.WriteLine("向服务器发送消息：{0}", sendMessage);
                //     }
                //     catch (Exception e)
                //     {
                //         Console.WriteLine(e);
                //         break;
                //     }
                // }
            };

            client.Connect();
            // Thread.Sleep(4000);
            // client.Disconnect();
            Console.ReadLine();
        }
    }
}
