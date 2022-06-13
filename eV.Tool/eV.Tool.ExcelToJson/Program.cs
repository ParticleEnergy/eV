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
using eV.Module.EasyLog;
using eV.Tool.ExcelToJson.Core;
using eV.Tool.ExcelToJson.Define;
using eV.Tool.ExcelToJson.Model;
using Microsoft.Extensions.Configuration;
ConfigurationBuilder builder = new();
builder.AddJsonFile("appsettings.json");
var config = builder.Build();

if (config.GetSection(Const.InExcelPath).Value is "" or null)
{
    Logger.Error("");
    return;
}
if (config.GetSection(Const.OutObjectFilePath).Value is "" or null)
{
    Logger.Error("");
    return;
}
if (config.GetSection(Const.OutJsonFilePath).Value is "" or null)
{
    Logger.Error("");
    return;
}
if (config.GetSection(Const.OutObjectNamespace).Value is "" or null)
{
    Logger.Error("");
    return;
}
if (config.GetSection(Const.OutObjectFileHead).Value is "" or null)
{
    Logger.Error("");
    return;
}

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
{
    Parser parser = new(config);
    parser.OutClass(excel);
}




