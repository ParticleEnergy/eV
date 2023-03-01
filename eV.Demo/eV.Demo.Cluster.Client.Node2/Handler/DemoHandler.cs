using eV.Demo.Cluster.Client.Node2.Message;
using eV.Framework.Unity.Base;
using eV.Module.EasyLog;
using eV.Module.Routing.Attributes;
using eV.Module.Routing.Interface;

namespace eV.Demo.Cluster.Client.Node2.Handler;

[ReceiveMessageHandler]
public class DemoHandler : HandlerBase<SMessage>
{
    protected override Task Handle(ISession session, SMessage content)
    {
        Logger.Info(content.Text);
        return Task.CompletedTask;
    }
}
