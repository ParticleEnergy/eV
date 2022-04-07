using eV.Routing.Attributes;
namespace eV.DataStruct.ClientStruct;

[ClientMessage]
public class HelloClient
{
    public string? Text { get; set; }
}
