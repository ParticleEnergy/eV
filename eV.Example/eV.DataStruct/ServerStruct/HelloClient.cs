using eV.Routing.Attributes;
namespace eV.DataStruct.ServerStruct;

[ServerMessage]
public class HelloServer
{
    public string? Text { get; set; }
}
