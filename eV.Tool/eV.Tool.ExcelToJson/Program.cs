// See https://aka.ms/new-console-template for more information


using eV.Module.EasyLog;
using eV.Tool.ExcelToJson.Core;
using eV.Tool.ExcelToJson.Define;
using Microsoft.Extensions.Configuration;
using File = eV.Tool.ExcelToJson.Core.File;

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

var excelInfos = File.GetFiles(config.GetSection(Const.InExcelPath).Value);
var tableInfos = Excel.GetTableInfos(excelInfos);
if (tableInfos == null)
{
    return;
}

ParserData parser = new(config);
parser.GetTableData(tableInfos);

return;
if (args.Length > 0 && args[0].Equals("object"))
{
    ParserStruct parserStruct = new(config);
    File.InitOutClassPath(config.GetSection(Const.OutObjectFilePath).Value);
    parserStruct.OutClass(tableInfos, File.Write);
}
else
{
    ParserData parserData = new(config);
    File.InitOutJsonPath(config.GetSection(Const.OutJsonFilePath).Value);
    // parserData.OutJson(tableInfos, File.Write);
}

