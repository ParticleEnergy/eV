using eV.Module.Routing.Attributes;

namespace eV.Demo.Server.Message;

[ServerMessage]
public class SMessage
{
    public string Text { get; set; } = string.Empty;
}
