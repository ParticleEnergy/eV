using eV.Module.Routing.Attributes;

namespace eV.Demo.Cluster.Server.Node1.Message;

[ServerMessage]
public class SMessage
{
    public string Text { get; set; } = string.Empty;
}
