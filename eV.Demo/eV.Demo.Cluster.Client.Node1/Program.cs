// See https://aka.ms/new-console-template for more information

using eV.Demo.Cluster.Client.Node1.Message;
using eV.Framework.Unity;
using eV.Module.Routing.Interface;

UnitySetting setting = new() { Host = "127.0.0.1", Port = 8001, ProjectAssemblyString = "eV.Demo.Cluster.Client.Node1", PublicObjectAssemblyString = "eV.Demo.Cluster.Client.Node1" };
Client client = new(setting);
client.OnConnect += delegate(ISession session)
{
    CMessage message = new() { Text = "Client.Node1" };
    session.Send(message);
};
client.Connect();
Console.ReadLine();
