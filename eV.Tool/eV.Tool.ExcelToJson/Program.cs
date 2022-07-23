// See https://aka.ms/new-console-template for more information


using eV.Module.EasyLog;
using eV.Tool.ExcelToJson.Core;
using eV.Tool.ExcelToJson.Define;
using eV.Tool.ExcelToJson.Utils;
using Microsoft.Extensions.Configuration;
string configFile = args.Length switch
{
    > 0 when args[0].Equals("prodServer") => "appsettings.prod.server.json",
    > 0 when args[0].Equals("prodUnity") => "appsettings.prod.unity.json",
    > 0 when args[0].Equals("devServer") => "appsettings.dev.server.json",
    > 0 when args[0].Equals("devUnity") => "appsettings.dev.unity.json",
    _ => "appsettings.json"
};

ConfigurationBuilder builder = new();
builder.AddJsonFile(configFile);
var config = builder.Build();

if (config.GetSection(Const.InExcelPath).Value is "" or null)
{
    Logger.Error($"{configFile} missing InExcelPath");
    return;
}
if (config.GetSection(Const.OutObjectFilePath).Value is "" or null)
{
    Logger.Error($"{configFile} missing OutObjectFilePath");
    return;
}
if (config.GetSection(Const.OutJsonFilePath).Value is "" or null)
{
    Logger.Error($"{configFile} missing OutJsonFilePath");
    return;
}
if (config.GetSection(Const.OutObjectNamespace).Value is "" or null)
{
    Logger.Error($"{configFile} missing OutObjectNamespace");
    return;
}
if (config.GetSection(Const.OutObjectFileHead).Value is "" or null)
{
    Logger.Error($"{configFile} missing OutObjectFileHead");
    return;
}
if (config.GetSection(Const.JsonFormatting).Value is "" or null)
{
    Logger.Error($"{configFile} missing JsonFormatting");
    return;
}
if (config.GetSection(Const.CSharpVersion).Value is "" or null)
{
    Logger.Error($"{configFile} missing CSharpVersion");
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
    case > 1 when args[1].Equals("object"):
        {
            FileUtils.InitOutClassPath(config.GetSection(Const.OutObjectFilePath).Value);
            analyticStructure.Generate();
            break;
        }
    case > 1 when args[1].Equals("json"):
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
