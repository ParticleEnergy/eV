using eV.Module.Routing.Attributes;

namespace eV.Demo.Cluster.Server.Node1.Message;

[ClientMessage]
public class CMessage
{
    public string Text { get; set; } = string.Empty;
}
