
using eV.Demo.Cluster.Server.Node2.Message;
using eV.Framework.Server.Base;
using eV.Module.Routing.Attributes;
using eV.Module.Routing.Interface;

namespace eV.Demo.Cluster.Server.Node2.Handler;

[ReceiveMessageHandler]
public class DemoHandler : HandlerBase<CMessage>
{
    public DemoHandler()
    {
        Skip = true;
    }

    protected override Task Handle(ISession session, CMessage content)
    {
        if (session.SessionId == null || session.SessionId.Equals(""))
            session.Activate("Server.Node2");

        session.Send(new SMessage { Text = "Server.Node2" });
        return Task.CompletedTask;
    }
}
