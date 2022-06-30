using eV.Module.Routing.Attributes;
namespace eV.PublicObject.ServerObject;

[ServerMessage]
public class ServerHelloMessage
{
    public string? Text { get; set; }
}
