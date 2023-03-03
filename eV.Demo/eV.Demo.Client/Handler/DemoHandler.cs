using eV.Demo.Client.Message;
using eV.Framework.Unity.Base;
using eV.Module.EasyLog;
using eV.Module.Routing.Attributes;
using eV.Module.Routing.Interface;

namespace eV.Demo.Client.Handler;

[ReceiveMessageHandler]
public class DemoHandler : HandlerBase<SMessage>
{
    protected override Task Handle(ISession session, SMessage content)
    {
        Logger.Info(content.Text);
        // session.Send(new CMessage { Text = content.Text });
        // Thread.Sleep(1000);
        return Task.CompletedTask;
    }
}
