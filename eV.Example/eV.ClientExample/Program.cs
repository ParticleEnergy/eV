// See https://aka.ms/new-console-template for more information


using eV.Framework.Client;
using eV.Module.Routing.Interface;
using eV.PublicObject.ClientObject;


// Profile.OnLoad += delegate
// {
//     Profile.AssignmentConfigObject(GameProfile.Instance);
// };
ClientSetting setting = new()
{
    Host = "127.0.0.1",
    Port = 8888,
    HandlerNamespace = "eV.ClientExample",
    DataStructNamespace = "eV.PublicObject",
    GameProfilePath = "/Users/three.zhang/Projects/CSharp/eV/eV.Example/eV.ClientExample/ProfileJson"
};
Client client = new(setting);
client.OnConnect += delegate(ISession session)
{
    ClientHelloMessage helloWorldClient = new()
    {
        Text = "Client: Hello world"
    };
    session.Send(helloWorldClient);
};
client.Connect();
Console.ReadLine();
