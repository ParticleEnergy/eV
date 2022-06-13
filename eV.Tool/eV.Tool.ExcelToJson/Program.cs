// See https://aka.ms/new-console-template for more information

// using System.Data;
// using eV.Tool.ExcelToJson.Core;
// using eV.Tool.ExcelToJson.Define;
// using eV.Tool.ExcelToJson.Model;
// using NPOI.SS.UserModel;
// using NPOI.XSSF.UserModel;
// // using File = eV.Tool.ExcelToJson.Core.File;
//
// // Console.WriteLine(File.GetFiles("/Users/three.zhang/Desktop").Count);
//
using eV.Tool.ExcelToJson.Core;
using eV.Tool.ExcelToJson.Model;
using Microsoft.Extensions.Configuration;
ConfigurationBuilder builder = new();
builder.AddJsonFile("appsettings.json");
var config = builder.Build();

Parser parser = new(config);



var excel = Excel.GetTableInfos(new List<ExcelInfo>
{
    new()
    {
        FileName = "Example",
        FilePath = "/Users/three.zhang/Desktop/Example.xlsx",
        FileType = "xlsx"
    }
});

if (excel != null)
    Console.WriteLine(excel[0].FileName);

// ObjectInfo objectInfo = new()
// {
//     NamespaceName = "eV.Profile",
//     ClassName = "Demo",
//     ProfileType = "List",
//     // ProfileDetailType = "string, "
// };
//
// string BaseProperty = string.Format(Template.BaseProperty, "string", "Name", "string.Empty");
// string ComplexProperty = string.Format(Template.ComplexProperty, "List<string>", "Names");
//
// objectInfo.IsMain = true;
// Console.WriteLine(objectInfo.ToString());

// List<string> abc = new List<string>()
// {
//     "1", "2","3"
// };
//
// Console.WriteLine(string.Join("\n", abc));

// Console.WriteLine(string.Format(Template.ProfileObject, objectInfo.NamespaceName, objectInfo.NamespaceName, objectInfo.ClassName, objectInfo.ProfileType, objectInfo.ProfileDetailType,objectInfo.ClassName, objectInfo.ClassName, BaseProperty));
