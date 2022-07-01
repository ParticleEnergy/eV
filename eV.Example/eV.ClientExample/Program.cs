// See https://aka.ms/new-console-template for more information


using eV.ClientExample.Handler;
using eV.Framework.Client;
using eV.Framework.Unity;
using eV.Module.EasyLog;
using eV.Module.Routing.Interface;
using eV.PublicObject.ClientObject;
using eV.PublicObject.ServerObject;
using Client = eV.Framework.Client.Client;


// Profile.OnLoad += delegate
// {
//     Profile.AssignmentConfigObject(GameProfile.Instance);
// };
ClientSetting setting = new()
{
    Host = "127.0.0.1",
    Port = 8888,
    HandlerAssemblyString = "eV.ClientExample",
    PublicObjectAssemblyString = "eV.PublicObject",
    GameProfilePath = "/Users/three.zhang/Projects/CSharp/eV/eV.Example/eV.ClientExample/ProfileJson"
};
Client client = new(setting);
client.OnConnect += delegate(ISession session)
{
    ClientHelloMessage helloWorldClient = new()
    {
        Text = "Server: Hello world"
    };
    session.Send(helloWorldClient);
};

var helloHandler = HandlerManager.GetHandler<HelloHandler>();

if (helloHandler != null)
    helloHandler.Handler += delegate(ISession session, ServerHelloMessage content)
    {
        Logger.Info("content.Text!");
    };
client.Connect();
Console.ReadLine();
