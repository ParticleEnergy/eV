// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Text;
using eV.Network.Core.Interface;
using eV.Network.Udp;


// Service service = new(new ServiceSetting
// {
//     ListenPort = 1234,
//     CommPort = 8888
// });
//
// service.OnBind += delegate(IUdpChannel channel)
// {
//     channel.SendBroadcast(Encoding.Unicode.GetBytes("SendBroadcast"));
//     channel.SendMulticast(Encoding.Unicode.GetBytes("SendMulticast"));
//     channel.Send(Encoding.Unicode.GetBytes("Send"), new IPEndPoint(IPAddress.Parse("234.5.6.7"), 8888));
//
// };
//
// service.Start();
//
// Console.ReadLine();
//


string b = Dns.GetHostName();
string d = Dns.GetHostEntry("localhost").HostName;

Console.WriteLine(b);
Console.WriteLine(d);


// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using eV.Network.Udp;
// var server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
// server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
//
// SocketAsyncEventArgs socketAsyncEventArgs = new ();
//
// string welcome = "测试"  + ",申请链接,";
// byte[] data = new byte[1024];
// data = Encoding.Unicode.GetBytes(welcome);
//
//
// socketAsyncEventArgs.SetBuffer(data, 0, data.Length);
// socketAsyncEventArgs.UserToken = server;
// socketAsyncEventArgs.RemoteEndPoint = new IPEndPoint(
//     IPAddress.Parse("127.0.0.1"),
//     8888
// );
//
// server.SendToAsync(socketAsyncEventArgs);
//
// Console.ReadLine();
//
//
// using System.Net;
// using System.Text;
// using eV.Network.Core.Interface;
// using eV.Network.Udp;
// ServiceSetting setting = new()
// {
//     Host = "192.168.2.108",
//     Port = 2222
// };
//
// Service service = new(setting);
//
// service.OnBind += delegate(IUdpChannel channel)
// {
//     var data = Encoding.Unicode.GetBytes("123");
//     channel.Send(data, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888 ));
//     // channel.Send(data, new IPEndPoint(IPAddress.Broadcast, 8888 ));
//
//     // channel.SendBroadcast(data);
// };
//
// service.OnRelease += delegate
// {
//
// };
// service.Start();
//
// Console.ReadLine();

//
//
// using System.Net;
// using System.Net.Sockets;
// using eV.ClientUdpExample;
// var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
// socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, false);
//
// // 广播
// // socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
// //设置多播
//
// // var multicastOption = new MulticastOption(IPAddress.Parse("234.5.6.7"), IPAddress.Parse("192.168.2.108"));
//
// // socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, multicastOption);
// // IP 多路广播生存时间
// // socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 10);
//
// // socket.Bind(new IPEndPoint(IPAddress.Parse("192.168.2.108"), 8888));
//
// var ip = new IPEndPoint(IPAddress.Parse("234.5.6.111"), 8888);
// // var ip = new IPEndPoint(IPAddress.Parse("192.168.2.108"), 8888);
// // var ip = new IPEndPoint(IPAddress.Broadcast, 8888);
// Send send = new(socket, ip);
// send.send();
// Console.ReadLine();
