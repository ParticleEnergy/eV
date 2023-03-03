using eV.Demo.Server.Message;
using eV.Demo.Server.Service;
using eV.Framework.Server.Base;
using eV.Module.Routing.Attributes;
using eV.Module.Routing.Interface;

namespace eV.Demo.Server.Handler;

[ReceiveMessageHandler]
public class DemoHandler : HandlerBase<CMessage>
{
    public DemoHandler()
    {
        Skip = true;
    }

    protected override async Task Handle(ISession session, CMessage content)
    {
        DemoService demoService = new();

        if (session.SessionId == null || session.SessionId.Equals(""))
            session.Activate("Server");

        session.Send(new SMessage { Text = "Server" });
        await demoService.Produce(new QMessage{Text = content.Text});
    }
}
