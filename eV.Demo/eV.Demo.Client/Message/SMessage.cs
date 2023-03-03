using eV.Module.Routing.Attributes;

namespace eV.Demo.Client.Message;

[ServerMessage]
public class SMessage
{
    public string Text { get; set; } = string.Empty;
}
