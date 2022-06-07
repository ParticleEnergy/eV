// See https://aka.ms/new-console-template for more information

using eV.Framework.Server;
using eV.Module.EasyLog;
using eV.ServerExample;
new Application(args).SetProfile(GameProfile.Instance).SetServerOnConnected(delegate { Logger.Info("SetServerOnConnected"); }).SetServerSessionOnActivate(delegate { Logger.Info("SetServerSessionOnActivate"); }).SetServerOnConnected(delegate { Logger.Info("SetServerOnConnected"); }).Run();
