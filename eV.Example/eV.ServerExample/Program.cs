// See https://aka.ms/new-console-template for more information


using eV.Module.GameProfile;
using eV.Module.Routing.Interface;
using eV.Framework.Server;
using eV.ServerExample;
Profile.OnLoad += delegate
{
    Profile.AssignmentConfigObject(GameProfile.Instance);
};
Server server = new();
server.OnConnected += delegate(ISession session)
{

};
server.SessionOnActivate += delegate(ISession session)
{

};
server.SessionOnRelease += delegate(ISession session)
{

};
server.Start();

Console.ReadLine();
