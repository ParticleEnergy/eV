using eV.Demo.Server.Message;
using eV.Framework.Server.Base;
using eV.Module.EasyLog;
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

    protected override Task Handle(ISession session, CMessage content)
    {
        Logger.Info(content.Text);
        if (session.SessionId == null || session.SessionId.Equals(""))
            session.Activate(Guid.NewGuid().ToString());

        session.Send(new SMessage { Text = content.Text });
        return Task.CompletedTask;
    }
}
