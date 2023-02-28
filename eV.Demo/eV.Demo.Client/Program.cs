// See https://aka.ms/new-console-template for more information

using eV.Demo.Client.Message;
using eV.Framework.Unity;
using eV.Module.Routing.Interface;

UnitySetting setting = new()
{
    Host = "127.0.0.1",
    Port = 8001,
    ProjectAssemblyString = "eV.Demo.Client",
    PublicObjectAssemblyString = "eV.Demo.Client",
};
Client client = new(setting);
client.OnConnect += delegate(ISession session)
{
    CMessage message = new()
    {
        Text = "hello"
    };
    session.Send(message);
};
client.Connect();
Console.ReadLine();
