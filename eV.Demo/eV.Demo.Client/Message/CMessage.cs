using eV.Module.Routing.Attributes;

namespace eV.Demo.Client.Message;

[ClientMessage]
public class CMessage
{
    public string Text { get; set; } = string.Empty;
}
