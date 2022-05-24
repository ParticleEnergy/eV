using eV.Module.Routing.Attributes;
namespace eV.PublicObject.ServerObject;

[ServerMessage]
public class HelloServerMessage
{
    public string? Text { get; set; }
}
