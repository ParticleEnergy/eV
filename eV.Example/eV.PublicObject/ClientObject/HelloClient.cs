using eV.Routing.Attributes;
namespace eV.PublicObject.ClientObject;

[ClientMessage]
public class HelloClientMessage
{
    public string? Text { get; set; }
}