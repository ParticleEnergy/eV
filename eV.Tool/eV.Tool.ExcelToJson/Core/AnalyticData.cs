// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Globalization;
using eV.Module.EasyLog;
using eV.Tool.ExcelToJson.Define;
using eV.Tool.ExcelToJson.Excel;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NPOI.SS.UserModel;

namespace eV.Tool.ExcelToJson.Core;

public class AnalyticData
{
    public Action<string, string>? Write { get; set; }
    private readonly IConfigurationSection _configuration;
    private readonly List<ExcelInfo> _excelInfos;

    public AnalyticData(IConfigurationSection configuration, List<ExcelInfo> excelInfos)
    {
        _configuration = configuration;
        _excelInfos = excelInfos;
    }

    public void Generate()
    {
        foreach (ExcelInfo excelInfo in _excelInfos)
        {
            if (excelInfo.MainSheetInfo == null)
                continue;

            Dictionary<int, List<KeyValuePair<SheetInfo, Dictionary<string, object>?>>> data = new();
            for (int i = excelInfo.SubSheetInfos.Count - 1; i >= 0; i--)
            {
                SheetInfo sheetInfo = excelInfo.SubSheetInfos[i];
                data.TryGetValue(sheetInfo.Hierarchy.Count,
                    out List<KeyValuePair<SheetInfo, Dictionary<string, object>?>>? flag);
                if (flag == null)
                {
                    data[sheetInfo.Hierarchy.Count] = new List<KeyValuePair<SheetInfo, Dictionary<string, object>?>>();
                }

                data.TryGetValue(sheetInfo.Hierarchy.Count + 1,
                    out List<KeyValuePair<SheetInfo, Dictionary<string, object>?>>? d);
                data[sheetInfo.Hierarchy.Count].Add(KeyValuePair.Create(sheetInfo, GetSubSheetData(sheetInfo, d)));
            }

            data.TryGetValue(0, out List<KeyValuePair<SheetInfo, Dictionary<string, object>?>>? subData);

            object? mainTable = GetMainSheetData(excelInfo.MainSheetInfo, subData);
            if (mainTable == null)
                continue;

            string jsonString = _configuration.GetSection(Const.JsonFormatting).Value.Equals("Indented")
                ? JsonConvert.SerializeObject(mainTable, Formatting.Indented)
                : JsonConvert.SerializeObject(mainTable);
            string path = _configuration.GetSection(Const.OutJsonFilePath).Value;
            string file = $"{path}/{excelInfo.FileName}Profile.json";
            Write?.Invoke(file, jsonString);
        }
    }

    private static Dictionary<string, object>? GetSubSheetData(SheetInfo sheetInfo,
        List<KeyValuePair<SheetInfo, Dictionary<string, object>?>>? data)
    {
        Dictionary<string, object> result = new();

        if (sheetInfo.PrimaryKeyFieldInfo == null)
        {
            Logger.Error($"Sheet: {sheetInfo.FullName} primary key cannot be empty");
            return null;
        }

        if (sheetInfo.ForeignKeyFieldInfo == null)
        {
            Logger.Error($"Sheet: {sheetInfo.FullName} foreign key cannot be empty");
            return null;
        }

        for (int i = 0; i < sheetInfo.Data.Count; i++)
        {
            var row = sheetInfo.Data[i];
            var dataRow = GetRow(row, sheetInfo);

            if (dataRow == null)
                continue;

            string pk = dataRow[sheetInfo.PrimaryKeyFieldInfo.Name]?.ToString() ?? "";
            if (pk.Equals(""))
            {
                Logger.Error($"Sheet: {sheetInfo.FullName} Row: {i + 1} primary key cannot be empty");
                continue;
            }

            string fk = row.GetCell(sheetInfo.ForeignKeyFieldInfo.Index)?.ToString() ?? "";
            if (fk.Equals(""))
            {
                Logger.Error($"Sheet: {sheetInfo.FullName} Row: {i + 1} foreign key cannot be empty");
                continue;
            }

            if (data != null)
            {
                foreach ((SheetInfo subSheetInfo, Dictionary<string, object>? d)in data)
                {
                    if (d != null && string.Join("@", subSheetInfo.Hierarchy).Equals(sheetInfo.FullName))
                    {
                        d.TryGetValue(pk, out object? value);
                        switch (subSheetInfo.ForeignKeyFieldInfo!.Type)
                        {
                            case FieldType.ForeignKeyDictionary:
                                dataRow[$"{subSheetInfo.Name}Dictionary"] = value;
                                break;
                            case FieldType.ForeignKeyObject:
                                dataRow[$"{subSheetInfo.Name}"] = value;
                                break;
                            case FieldType.ForeignKeyList:
                                dataRow[$"{subSheetInfo.Name}List"] = value;
                                break;
                            default:
                                Logger.Error(
                                    $"Sub Sheet: {subSheetInfo.FullName} Row: {i + 1} foreign key cannot be empty");
                                break;
                        }
                    }
                }
            }

            switch (sheetInfo.ForeignKeyFieldInfo.Type)
            {
                case FieldType.ForeignKeyDictionary:
                    {
                        result.TryGetValue(fk, out object? flag);
                        if (flag == null)
                        {
                            result[fk] = new Dictionary<string, object>();
                        }

                        ((Dictionary<string, object>)result[fk])[pk] = dataRow;
                        break;
                    }
                case FieldType.ForeignKeyList:
                    {
                        result.TryGetValue(fk, out object? flag);
                        if (flag == null)
                        {
                            result[fk] = new List<object>();
                        }

                        ((List<object>)result[fk]).Add(dataRow);
                        break;
                    }
                default:
                    result[fk] = dataRow;
                    break;
            }
        }

        return result;
    }

    private static object? GetMainSheetData(SheetInfo sheetInfo,
        List<KeyValuePair<SheetInfo, Dictionary<string, object>?>>? subData)
    {
        Dictionary<string, Dictionary<string, object?>> dataDictionary = new();
        List<Dictionary<string, object?>> dataList = new();

        if (sheetInfo.PrimaryKeyFieldInfo == null)
            return null;


        foreach (var dataRow in sheetInfo.Data.Select(row => GetRow(row, sheetInfo)))
        {
            if (dataRow == null)
                continue;

            if (sheetInfo.PrimaryKeyFieldInfo == null)
                continue;

            string pk = dataRow[sheetInfo.PrimaryKeyFieldInfo.Name]?.ToString() ?? "";
            if (pk.Equals(""))
                continue;

            if (subData != null)
            {
                foreach ((SheetInfo subSheetInfo, Dictionary<string, object>? data)in subData)
                {
                    if (data == null)
                        continue;

                    if (subSheetInfo.ForeignKeyFieldInfo == null)
                        continue;

                    data.TryGetValue(pk, out object? value);
                    switch (subSheetInfo.ForeignKeyFieldInfo.Type)
                    {
                        case FieldType.ForeignKeyDictionary:
                            dataRow[$"{subSheetInfo.Name}Dictionary"] = value;
                            break;
                        case FieldType.ForeignKeyObject:
                            dataRow[$"{subSheetInfo.Name}"] = value;
                            break;
                        case FieldType.ForeignKeyList:
                            dataRow[$"{subSheetInfo.Name}List"] = value;
                            break;
                        default:
                            Logger.Error($"Sub Sheet: {subSheetInfo.FullName} foreign key cannot be empty");
                            break;
                    }
                }
            }

            if (sheetInfo.PrimaryKeyFieldInfo.Type.Equals(FieldType.PrimaryKeyDictionary))
            {
                dataDictionary[pk] = dataRow;
            }
            else
            {
                dataList.Add(dataRow);
            }
        }

        return sheetInfo.PrimaryKeyFieldInfo?.Type.Equals(FieldType.PrimaryKeyDictionary) == true
            ? dataDictionary
            : dataList;
    }

    private static Dictionary<string, object?>? GetRow(IRow row, SheetInfo sheetInfo)
    {
        Dictionary<string, object?> dataRow = new();

        if (sheetInfo.PrimaryKeyFieldInfo == null)
            return null;

        if (sheetInfo.FieldInfos.Count <= 0)
            return null;

        string pk = row.GetCell(sheetInfo.PrimaryKeyFieldInfo.Index)?.ToString() ?? "";
        if (pk.Equals(""))
        {
            Logger.Error($"Sheet: {sheetInfo.FullName} primary key cannot be empty");
            return null;
        }

        dataRow[sheetInfo.PrimaryKeyFieldInfo.Name] = pk;

        foreach (FieldInfo fieldInfo in sheetInfo.FieldInfos)
        {
            try
            {
                string originValue;
                var cell = row.GetCell(fieldInfo.Index);

                if (cell == null)
                {
                    originValue = "";
                }
                else
                {
                    if (cell.CellType == CellType.Formula)
                    {
                        originValue = fieldInfo.Type switch
                        {
                            FieldType.Bool => cell.BooleanCellValue.ToString(),
                            FieldType.Int => cell.NumericCellValue.ToString(CultureInfo.InvariantCulture),
                            FieldType.Double => cell.NumericCellValue.ToString(CultureInfo.InvariantCulture),
                            _ => cell.StringCellValue
                        };
                    }
                    else
                    {
                        originValue = cell.ToString() ?? "";
                    }
                }

                if (fieldInfo.Type.StartsWith(FieldType.Dict) || fieldInfo.Type.StartsWith(FieldType.List))
                {
                    if (originValue.Equals(""))
                    {
                        dataRow[fieldInfo.Name] = null;
                    }
                    else
                    {
                        dataRow[fieldInfo.Name] = JsonConvert.DeserializeObject(originValue);
                    }
                }
                else
                {
                    dataRow[fieldInfo.Name] = fieldInfo.Type switch
                    {
                        FieldType.String => originValue,
                        FieldType.Int => originValue.Equals("") ? 0 : Convert.ToInt32(originValue),
                        FieldType.Double => originValue.Equals("") ? 0 : Convert.ToDouble(originValue),
                        FieldType.Bool => originValue.ToUpper() == "TRUE",
                        _ => null
                    };
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Sheet: {sheetInfo.FullName} Field: {fieldInfo.Name} {e.Message}", e);
                return null;
            }
        }

        return dataRow;
    }
}
