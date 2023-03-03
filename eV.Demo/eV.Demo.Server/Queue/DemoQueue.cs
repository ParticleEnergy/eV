using eV.Demo.Server.Message;
using eV.Module.EasyLog;
using eV.Module.Queue.Attributes;
using eV.Module.Queue.Base;

namespace eV.Demo.Server.Queue;

[QueueConsumer]
public class DemoQueue : QueueBase<QMessage>
{
    protected override Task<bool> Consume(QMessage data)
    {
        Logger.Info(data.Text);
        return Task.FromResult(true);
    }
}
