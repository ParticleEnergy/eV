// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using System.Data;
using eV.Module.EasyLog;
using eV.Tool.ExcelToJson.Define;
using eV.Tool.ExcelToJson.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using FieldInfo = eV.Tool.ExcelToJson.Model.FieldInfo;
namespace eV.Tool.ExcelToJson.Core;

public class ParserData
{
    private readonly IConfigurationRoot _configuration;
    public ParserData(IConfigurationRoot configuration)
    {
        _configuration = configuration;
    }

    public void GetTableData(List<TableInfo> tableInfos)
    {

        foreach (TableInfo tableInfo in tableInfos)
        {
            Dictionary<string, Dictionary<string, object>?> data = new();

            object? mainTable = GetMainSheetData(tableInfo.MainSheet);
            if (mainTable == null)
            {
                return;
            }
            foreach (SheetInfo sheetInfo in tableInfo.SubSheetInfos)
            {
                data[sheetInfo.FullName] = GetSubSheetData(sheetInfo);
            }

            if (tableInfo.MainSheet.PrimaryKeyFieldInfo != null)
            {
                if (tableInfo.MainSheet.PrimaryKeyFieldInfo.Type.Equals(FieldType.PrimaryKeyList))
                {
                    foreach (var mainData in (List<Dictionary<string, object>>)mainTable)
                    {

                    }
                }
                else
                {
                    foreach ((string _, var mainData) in (Dictionary<string, Dictionary<string, object>>)mainTable)
                    {

                    }
                }
            }
            string jsonString = JsonConvert.SerializeObject(mainTable);
            Console.WriteLine(jsonString);
        }
    }

    private static Dictionary<string, object>? GetSubSheetData(SheetInfo sheetInfo)
    {
        Dictionary<string, object> result = new();

        if (sheetInfo.ForeignKeyFieldInfo == null)
        {
            Logger.Error($"{sheetInfo.FullName} Foreign key cannot be empty");
            return null;
        }

        foreach (var row in sheetInfo.Data)
        {
            var dataRow = GetRow(row, sheetInfo);
            if (dataRow == null)
            {
                return null;
            }
            if (sheetInfo.PrimaryKeyFieldInfo != null)
            {
                if (row.GetCell(sheetInfo.PrimaryKeyFieldInfo.Index).ToString() == null)
                {
                    Logger.Error($"{sheetInfo.FullName} Primary key cannot be empty");
                    return null;
                }
                dataRow[sheetInfo.PrimaryKeyFieldInfo.Name] = row.GetCell(sheetInfo.PrimaryKeyFieldInfo.Index).ToString() ?? "";
            }
            string? fkValue = row.GetCell(sheetInfo.ForeignKeyFieldInfo.Index).ToString();
            if (fkValue == null)
            {
                Logger.Error($"{sheetInfo.FullName} Foreign key cannot be empty");
                return null;
            }

            switch (sheetInfo.ForeignKeyFieldInfo.Type)
            {
                case FieldType.ForeignKeyDictionary when sheetInfo.PrimaryKeyFieldInfo == null:
                    Logger.Error($"{sheetInfo.FullName} Primary key cannot be empty");
                    return null;
                case FieldType.ForeignKeyDictionary:
                    {
                        string? pk = row.GetCell(sheetInfo.PrimaryKeyFieldInfo.Index).ToString();
                        if (pk == null)
                        {
                            Logger.Error($"{sheetInfo.FullName} Primary key cannot be empty");
                            return null;
                        }

                        result.TryGetValue(fkValue, out object? flag);
                        if (flag == null)
                        {
                            result[fkValue] = new Dictionary<string, object>();
                        }
                        ((Dictionary<string, object>)result[fkValue])[pk] = dataRow;
                        break;
                    }
                case FieldType.ForeignKeyList:
                    {
                        result.TryGetValue(fkValue, out object? flag);
                        if (flag == null)
                        {
                            result[fkValue] = new List<object>();
                        }
                        ((List<object>)result[fkValue]).Add(dataRow);
                        break;
                    }
                default:
                    result[fkValue] = dataRow;
                    break;
            }
        }
        return result;
    }

    private static object? GetMainSheetData(SheetInfo sheetInfo)
    {
        Dictionary<string, Dictionary<string, object>> dataDictionary = new();
        List<Dictionary<string, object>> dataList = new();
        foreach (var dataRow in sheetInfo.Data.Select(row => GetMainRow(row, sheetInfo)))
        {
            if (dataRow == null)
            {
                return null;
            }

            if (sheetInfo.PrimaryKeyFieldInfo != null)
            {
                if (sheetInfo.PrimaryKeyFieldInfo.Type.Equals(FieldType.PrimaryKeyDictionary))
                {
                    dataDictionary[dataRow[sheetInfo.PrimaryKeyFieldInfo.Name].ToString()!] = dataRow;
                }
                else
                {
                    dataList.Add(dataRow);
                }
            }
            else
            {
                dataList.Add(dataRow);
            }
        }

        if (sheetInfo.PrimaryKeyFieldInfo == null)
            return dataList;
        if (sheetInfo.PrimaryKeyFieldInfo.Type.Equals(FieldType.PrimaryKeyDictionary))
        {
            return dataDictionary;
        }
        return dataList;
    }

    private static Dictionary<string, object>? GetMainRow(IRow row, SheetInfo sheetInfo)
    {
        Dictionary<string, object>? dataRow = GetRow(row, sheetInfo);
        if (dataRow == null)
        {
            return null;
        }

        if (sheetInfo.PrimaryKeyFieldInfo == null)
            return dataRow;

        if (row.GetCell(sheetInfo.PrimaryKeyFieldInfo.Index).ToString() == null)
        {
            Logger.Error($"{sheetInfo.FullName} Primary key cannot be empty");
            return null;
        }
        dataRow[sheetInfo.PrimaryKeyFieldInfo.Name] = row.GetCell(sheetInfo.PrimaryKeyFieldInfo.Index).ToString() ?? "";
        return dataRow;
    }

    private static Dictionary<string, object>? GetRow(IRow row, SheetInfo sheetInfo)
    {
        Dictionary<string, object> dataRow = new();
        foreach (FieldInfo fieldInfo in sheetInfo.FieldInfos)
        {
            try
            {
                string originValue = row.GetCell(fieldInfo.Index).ToString() ?? "";
                dataRow[fieldInfo.Name] = fieldInfo.Type switch
                {
                    FieldType.String => originValue,
                    FieldType.Bool => originValue == "TRUE",
                    FieldType.Int => originValue.Equals("") ? 0 : Convert.ToInt32(originValue),
                    FieldType.Double => originValue.Equals("") ? 0 : Convert.ToDouble(originValue),
                    FieldType.ListString => originValue.Split(Const.SplitFlag).ToList(),
                    FieldType.ListBool => originValue.Split(Const.SplitFlag).Select(s => s == "TRUE").ToList(),
                    FieldType.ListDouble => originValue.Split(Const.SplitFlag).Select(a => a.Equals("") ? 0 : Convert.ToDouble(a)).ToList(),
                    FieldType.ListInt => originValue.Split(Const.SplitFlag).Select(a => a.Equals("") ? 0 : Convert.ToInt32(a)).ToList(),
                    _ => dataRow[fieldInfo.Name]
                };
            }
            catch (Exception e)
            {
                Logger.Error($"{sheetInfo.FullName} field: {fieldInfo.Name}", e);
                return null;
            }
        }
        return dataRow;
    }
}
