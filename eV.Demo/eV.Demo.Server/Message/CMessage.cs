using eV.Module.Routing.Attributes;

namespace eV.Demo.Server.Message;

[ClientMessage]
public class CMessage
{
    public string Text { get; set; } = string.Empty;
}
