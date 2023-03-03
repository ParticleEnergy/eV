using eV.Module.Routing.Attributes;

namespace eV.Demo.Cluster.Client.Node1.Message;

[ServerMessage]
public class SMessage
{
    public string Text { get; set; } = string.Empty;
}
