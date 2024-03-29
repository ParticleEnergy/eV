﻿// See https://aka.ms/new-console-template for more information

using eV.Demo.Client.Message;
using eV.Framework.Unity;
using eV.Module.Routing.Interface;

string path = "/Users/three.zhang/Projects/eV/eV/eV.Tool/eV.Tool.GenerateCertificateFile/bin/Release/net6.0/certificate.pfx"; // 文件路径
byte[] pfxBytes = File.ReadAllBytes(path);

UnitySetting setting = new()
{
    Host = "127.0.0.1",
    TcpKeepAlive = true,
    Port = 8000,
    ProjectAssemblyString = "eV.Demo.Client",
    PublicObjectAssemblyString = "eV.Demo.Client",
    TlsTargetHost = "127.0.0.1",
    TlsCertPassword = "123456",
    TlsCertData = pfxBytes,
    TlsCheckCertificateRevocation = false
};

Client client = new(setting);
client.OnConnect += delegate(ISession session)
{
    CMessage message = new() { Text = "hello" };
    session.Send(message);
};
client.Connect();
Console.ReadLine();


//
//
// using StackExchange.Redis;
//
// ConfigurationOptions config = new() { Password = "123456", };
// config.EndPoints.Add("127.0.0.1", Convert.ToInt32(6379));
//
// // 创建 Redis 连接
// var redis = ConnectionMultiplexer.Connect(config);
//
// // 获取 Redis 数据库
// var db = redis.GetDatabase();
//
// string stream = "eV:Queue:eV.Demo.Server:Stream:QMessage";
// string group = "eV:Queue:eV.Demo.Server:Group:QMessage";
// string consumer = "eV:Queue:eV.Demo.Server:Node:ff7c1286-40c1-4542-898c-a8c34f7650ab:Consumer:QMessage";
// // 创建消费者组
// // await db.StreamCreateConsumerGroupAsync("eV:Queue:eV.Demo.Server:Stream:QMessage", "eV:Queue:eV.Demo.Server:Group:QMessage", StreamPosition.NewMessages);
//
// db.StreamAdd("eV:Queue:eV.Demo.Server:Stream:QMessage", "data", "111");
// db.StreamAdd("eV:Queue:eV.Demo.Server:Stream:QMessage", "data", "222");
// db.StreamAdd("eV:Queue:eV.Demo.Server:Stream:QMessage", "data", "333");
//
// var pendingInfo = db.StreamPending("eV:Queue:eV.Demo.Server:Stream:QMessage", "eV:Queue:eV.Demo.Server:Group:QMessage");
//
// // Console.WriteLine(pendingInfo.PendingMessageCount);
// // Console.WriteLine(pendingInfo.LowestPendingMessageId);
// // Console.WriteLine(pendingInfo.HighestPendingMessageId);
// // Console.WriteLine($"Consumer count: {pendingInfo.Consumers.Length}.");
// // // 读取消息
//
// string command = $"XREADGROUP GROUP {group} {consumer} BLOCK 0 COUNT 1 STREAMS {stream} >";
//
//
//
// while (true)
// {
//     Console.WriteLine(command);
//     var result =  redis.GetDatabase().Execute("XREADGROUP", new object[]{ "GROUP", group, consumer, "BLOCK", 0,"COUNT", 1, "STREAMS", stream, ">" });
//     // var messages =await  redis.GetDatabase().StreamReadGroupAsync(
//     //     "eV:Queue:eV.Demo.Server:Stream:QMessage",
//     //     "eV:Queue:eV.Demo.Server:Group:QMessage",
//     //     "eV:Queue:eV.Demo.Server:Node:ff7c1286-40c1-4542-898c-a8c34f7650ab:Consumer:QMessage",
//     //     ">",
//     //     count: 1,
//     //     true,
//     //     flags: CommandFlags.None);
//     //
//     //
//     // foreach (var message in messages)
//     // {
//     //     // 打印消息
//     //     Console.WriteLine($"Message received: {message["data"]}");
//     //
//     //     // 将消息标记为已处理
//     //     db.StreamAcknowledge("eV:Queue:eV.Demo.Server:Stream:QMessage", "eV:Queue:eV.Demo.Server:Group:QMessage", message.Id);
//     //     await db.StreamDeleteAsync("eV:Queue:eV.Demo.Server:Stream:QMessage", new []{message.Id});
//     // }
//     Console.WriteLine("test");
// }

// 建立 socket 连接并获取网络流
//
// using System.Net.Security;
// using System.Net.Sockets;
// using System.Text;
//
// Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
// await socket.ConnectAsync("example.com", 443);
// NetworkStream networkStream = new NetworkStream(socket, true);
//
// // 创建 SslStream 并使用网络流进行包装
// SslStream sslStream = new SslStream(networkStream);
// await sslStream.AuthenticateAsClientAsync("example.com");
//
// // 将 SslStream 封装到 SocketAsyncEventArgs 对象中
// SocketAsyncEventArgs socketArgs = new SocketAsyncEventArgs();
// socketArgs.RemoteEndPoint = socket.RemoteEndPoint;
// socketArgs.Completed += SocketArgs_Completed;
// socketArgs.UserToken = sslStream;
// socketArgs.SetBuffer(new byte[1024], 0, 1024);
//
// // 使用 SocketAsyncEventArgs 发送和接收数据
// await sslStream.WriteAsync(Encoding.UTF8.GetBytes("Hello, world!"));
// socket.SendAsync(socketArgs);
//
// // 接收响应数据
// await socket.ReceiveAsync(socketArgs);
// byte[] responseBuffer = socketArgs.Buffer;
// int bytesRead = socketArgs.BytesTransferred;
//
// // 解密响应数据
// string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
// Console.WriteLine(response);
