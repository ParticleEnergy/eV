// See https://aka.ms/new-console-template for more information


using eV.Module.EasyLog;
using eV.Tool.ExcelToJson.Core;
using eV.Tool.ExcelToJson.Define;
using eV.Tool.ExcelToJson.Utils;
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


var excelInfos = FileUtils.GetExcelInfos(config.GetSection(Const.InExcelPath).Value);
var analyticStructure = new AnalyticStructure(config, excelInfos)
{
    Write = FileUtils.Write
};

var analyticData = new AnalyticData(config, excelInfos)
{
    Write = FileUtils.Write
};


switch (args.Length)
{
    case > 0 when args[0].Equals("object"):
        {
            FileUtils.InitOutClassPath(config.GetSection(Const.OutObjectFilePath).Value);
            analyticStructure.Generate();
            break;
        }
    case > 0 when args[0].Equals("json"):
        {
            FileUtils.InitOutJsonPath(config.GetSection(Const.OutJsonFilePath).Value);
            analyticData.Generate();
            break;
        }
    default:
        {
            FileUtils.InitOutClassPath(config.GetSection(Const.OutObjectFilePath).Value);
            FileUtils.InitOutJsonPath(config.GetSection(Const.OutJsonFilePath).Value);

            analyticStructure.Generate();
            analyticData.Generate();
            break;
        }
}
// Lastest
