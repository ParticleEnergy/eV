// See https://aka.ms/new-console-template for more information


using eV.DataStruct.ClientStruct;
using eV.Routing.Interface;
using eV.Unity;


// Profile.OnLoad += delegate
// {
//     Profile.AssignmentConfigObject(GameProfile.Instance);
// };
UnitySetting setting = new()
{
    HandlerNamespace = "eV.ClientExample",
    DataStructNamespace = "eV.DataStruct",
    GameProfilePath = "/Users/three.zhang/Projects/CSharp/eV/eV.Example/eV.ClientExample/ProfileJson"
};
Client client = new(setting);
client.OnConnect += delegate(ISession session)
{
    // HelloClient helloWorldClient = new()
    // {
    //     Text = "测试测试"
    // };
    // session.Send(helloWorldClient);
};
client.Start();
Console.ReadLine();
