// See https://aka.ms/new-console-template for more information


using eV.Module.EasyLog;
using eV.Tool.ExcelToJson.Define;
using eV.Tool.ExcelToJson.Excel;
using Microsoft.Extensions.Configuration;

ConfigurationBuilder builder = new();
builder.AddJsonFile("appsettings.json");
var config = builder.Build();

if (config.GetSection(Const.InExcelPath).Value is "" or null)
{
    Logger.Error("appsettings.json missing InExcelPath");
    return;
}
if (config.GetSection(Const.OutObjectFilePath).Value is "" or null)
{
    Logger.Error("appsettings.json missing OutObjectFilePath");
    return;
}
if (config.GetSection(Const.OutJsonFilePath).Value is "" or null)
{
    Logger.Error("appsettings.json missing OutJsonFilePath");
    return;
}
if (config.GetSection(Const.OutObjectNamespace).Value is "" or null)
{
    Logger.Error("appsettings.json missing OutObjectNamespace");
    return;
}
if (config.GetSection(Const.OutObjectFileHead).Value is "" or null)
{
    Logger.Error("appsettings.json missing OutObjectFileHead");
    return;
}
if (config.GetSection(Const.JsonFormatting).Value is "" or null)
{
    Logger.Error("appsettings.json missing OutObjectFileHead");
    return;
}

var excelInfo = new ExcelInfo("/Users/three.zhang/Projects/BedroomAdventure/BedroomAdventure-config/Excel/Protagonist.xlsx","Protagonist.xlsx","xlsx");

// var excelInfos = File.GetFiles(config.GetSection(Const.InExcelPath).Value);
// var tableInfos = Excel.GetTableInfos(excelInfos);
// if (tableInfos == null)
// {
//     return;
// }
//
// switch (args.Length)
// {
//     case > 0 when args[0].Equals("object"):
//         {
//             ParserStruct parserStruct = new(config);
//             File.InitOutClassPath(config.GetSection(Const.OutObjectFilePath).Value);
//             parserStruct.OutClass(tableInfos, File.Write);
//             break;
//         }
//     case > 0 when args[0].Equals("json"):
//         {
//             ParserData parserData = new(config);
//             File.InitOutJsonPath(config.GetSection(Const.OutJsonFilePath).Value);
//             parserData.OutJson(tableInfos, File.Write);
//             break;
//         }
//     default:
//         {
//             ParserStruct parserStruct = new(config);
//             File.InitOutClassPath(config.GetSection(Const.OutObjectFilePath).Value);
//             parserStruct.OutClass(tableInfos, File.Write);
//             ParserData parserData = new(config);
//             File.InitOutJsonPath(config.GetSection(Const.OutJsonFilePath).Value);
//             parserData.OutJson(tableInfos, File.Write);
//             break;
//         }
// }
