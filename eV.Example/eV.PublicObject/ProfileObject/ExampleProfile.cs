// Head

using eV.Module.GameProfile.Attributes;
using eV.PublicObject.ProfileObject.Object;
namespace eV.PublicObject.ProfileObject;

[GameProfile]
public class ExampleProfile : Dictionary<string, ExampleRow>
{

}

public class ExampleRow
{
    /// <summary>
    /// 字符串
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// 整型
    /// </summary>
    public int Age { get; set; } = 0;
    /// <summary>
    /// 浮点
    /// </summary>
    public double Height { get; set; } = 0.0;
    /// <summary>
    /// 布尔
    /// </summary>
    public bool IsMan { get; set; } = false;
    /// <summary>
    /// 数组字符串
    /// </summary>
    public List<string>? Names { get; set; }
    /// <summary>
    /// 数组整型
    /// </summary>
    public List<int>? Ages { get; set; }
    /// <summary>
    /// 数组浮点
    /// </summary>
    public List<double>? Heights { get; set; }
    /// <summary>
    /// 数组布尔
    /// </summary>
    public List<bool>? IsMans { get; set; }
    /// <summary>
    /// One
    /// </summary>
    public One? One { get; set; }
}

