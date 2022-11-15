// See https://aka.ms/new-console-template for more information


using eV.Module.EasyLog;
using eV.Tool.ExcelToJson.Core;
using eV.Tool.ExcelToJson.Define;
using eV.Tool.ExcelToJson.Utils;
using Microsoft.Extensions.Configuration;

ConfigurationBuilder builder = new();
builder.AddJsonFile("appsettings.json");
var config = builder.Build();

foreach (IConfigurationSection configurationSection in config.GetChildren())
{
    if (configurationSection.GetSection(Const.InExcelPath).Value is "" or null)
    {
        Logger.Error($"{configurationSection.Key} missing InExcelPath");
        return;
    }

    if (configurationSection.GetSection(Const.OutObjectFilePath).Value is "" or null)
    {
        Logger.Error($"{configurationSection.Key} missing OutObjectFilePath");
        return;
    }

    if (configurationSection.GetSection(Const.OutJsonFilePath).Value is "" or null)
    {
        Logger.Error($"{configurationSection.Key} missing OutJsonFilePath");
        return;
    }

    if (configurationSection.GetSection(Const.OutObjectNamespace).Value is "" or null)
    {
        Logger.Error($"{configurationSection.Key} missing OutObjectNamespace");
        return;
    }

    if (configurationSection.GetSection(Const.OutObjectFileHead).Value is "" or null)
    {
        Logger.Error($"{configurationSection.Key} missing OutObjectFileHead");
        return;
    }

    if (configurationSection.GetSection(Const.JsonFormatting).Value is "" or null)
    {
        Logger.Error($"{configurationSection.Key} missing JsonFormatting");
        return;
    }

    if (configurationSection.GetSection(Const.CSharpVersion).Value is "" or null)
    {
        Logger.Error($"{configurationSection.Key} missing CSharpVersion");
        return;
    }

    var excelInfos = FileUtils.GetExcelInfos(configurationSection.GetSection(Const.InExcelPath).Value);

    var analyticStructure = new AnalyticStructure(configurationSection, excelInfos) { Write = FileUtils.Write };
    var analyticData = new AnalyticData(configurationSection, excelInfos) { Write = FileUtils.Write };

    FileUtils.InitOutClassPath(configurationSection.GetSection(Const.OutObjectFilePath).Value);
    FileUtils.InitOutJsonPath(configurationSection.GetSection(Const.OutJsonFilePath).Value);

    analyticStructure.Generate();
    analyticData.Generate();
}
