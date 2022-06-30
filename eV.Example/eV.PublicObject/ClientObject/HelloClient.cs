using eV.Module.Routing.Attributes;
namespace eV.PublicObject.ClientObject;

[ClientMessage]
public class ClientHelloMessage
{
    public string? Text { get; set; }
}
