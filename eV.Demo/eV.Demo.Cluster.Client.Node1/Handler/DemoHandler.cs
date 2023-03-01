using eV.Demo.Cluster.Client.Node1.Message;
using eV.Framework.Unity.Base;
using eV.Module.EasyLog;
using eV.Module.Routing.Attributes;
using eV.Module.Routing.Interface;

namespace eV.Demo.Cluster.Client.Node1.Handler;

[ReceiveMessageHandler]
public class DemoHandler : HandlerBase<SMessage>
{
    protected override Task Handle(ISession session, SMessage content)
    {
        Logger.Info(content.Text);
        session.Activate("Client.Node2");
        session.Send("Server.Node2",  new CMessage() { Text = "ClientNode1" });
        return Task.CompletedTask;
    }
}
