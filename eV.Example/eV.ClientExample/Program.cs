﻿// See https://aka.ms/new-console-template for more information


using eV.Client;
using eV.PublicObject.ClientObject;
using eV.Routing.Interface;


// Profile.OnLoad += delegate
// {
//     Profile.AssignmentConfigObject(GameProfile.Instance);
// };
ClientSetting setting = new()
{
    HandlerNamespace = "eV.ClientExample",
    DataStructNamespace = "eV.PublicObject",
    GameProfilePath = "/Users/three.zhang/Projects/CSharp/eV/eV.Example/eV.ClientExample/ProfileJson"
};
Client client = new(setting);
client.OnConnect += delegate(ISession session)
{
    HelloClientMessage helloWorldClient = new()
    {
        Text = "测试测试"
    };
    session.Send(helloWorldClient);
};
client.Start();
Console.ReadLine();
