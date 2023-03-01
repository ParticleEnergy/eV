using eV.Module.Routing.Attributes;

namespace eV.Demo.Cluster.Client.Node2.Message;

[ClientMessage]
public class CMessage
{
    public string Text { get; set; } = string.Empty;
}
