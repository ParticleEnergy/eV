// See https://aka.ms/new-console-template for more information


using eV.GameProfile;
using eV.Routing.Interface;
using eV.Server;
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
