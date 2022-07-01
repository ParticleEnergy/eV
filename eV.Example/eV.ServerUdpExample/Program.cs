// See https://aka.ms/new-console-template for more information


using System.Net;
using System.Text;
using eV.Network.Core.Interface;
using eV.Network.Udp;
Service service = new(new ServiceSetting());

service.OnBind += delegate(IUdpChannel channel)
{
    channel.Receive += delegate(byte[]? bytes, EndPoint? point)
    {
        Console.WriteLine(point);
        Console.WriteLine(Encoding.Unicode.GetString(bytes!));
    };
};

service.Start();

Console.ReadLine();


// var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
//
// // socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);
//
// // 广播
// socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
// // //设置多播
// socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);
// //
// var multicastOption = new MulticastOption(IPAddress.Parse("234.5.6.7"), IPAddress.Parse("192.168.2.108"));
// socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, multicastOption);
// // IP 多路广播生存时间
// socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 10);
// socket.Bind(new IPEndPoint(IPAddress.Any, 8888));
//
// Receive receive = new(socket);
//
// Console.ReadLine();
