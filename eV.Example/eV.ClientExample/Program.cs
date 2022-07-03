// See https://aka.ms/new-console-template for more information


using eV.ClientExample;
using eV.Framework.Unity;
using eV.Module.GameProfile;
using eV.Module.Routing.Interface;
using eV.PublicObject.ClientObject;

Profile.OnLoad += delegate
{
    Profile.AssignmentConfigObject(GameProfile.Instance);
};

UnitySetting setting = new()
{
    Host = "127.0.0.1",
    Port = 8888,
    ProjectAssemblyString = "eV.ClientExample",
    PublicObjectAssemblyString = "eV.PublicObject",
    GameProfilePath = "/Users/three.zhang/Projects/CSharp/eV/eV.Example/eV.PublicObject/Example/Profile/Json"
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

// var helloHandler = HandlerManager.GetHandler<HelloHandler>();
//
// if (helloHandler != null)
//     helloHandler.Handler += delegate(ISession session, ServerHelloMessage content)
//     {
//         // Logger.Info(GameProfile.Instance.ExampleProfile["2"].Name);
//         Logger.Info("content.Text!");
//     };
client.Connect();
Console.ReadLine();
