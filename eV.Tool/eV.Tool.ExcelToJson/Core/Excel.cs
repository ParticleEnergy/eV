// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.EasyLog;
using eV.Tool.ExcelToJson.Define;
using eV.Tool.ExcelToJson.Model;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
namespace eV.Tool.ExcelToJson.Core;

public static class Excel
{
    private const int CommentIndex = 0;
    private const int TypeIndex = 1;
    private const int NameIndex = 2;
    private const int DataStartIndex = 3;

    public static List<TableInfo>? GetTable(IEnumerable<ExcelInfo> excelInfos)
    {
        List<TableInfo> result = new();
        foreach (ExcelInfo excelInfo in excelInfos)
        {
            var workbook = GetWorkbook(excelInfo);
            if (workbook == null)
                continue;

            List<SheetInfo>? sheetInfos = GetSheet(workbook, excelInfo.FilePath);

            if (sheetInfos == null)
            {
                return null;
            }

            sheetInfos.Sort();
            result.Add(new TableInfo
            {
                FileName = excelInfo.FileName,
                FilePath = excelInfo.FilePath,
                SheetInfos = sheetInfos
            });
        }
        return result;
    }

    private static IWorkbook? GetWorkbook(ExcelInfo excelInfo)
    {
        try
        {
            IWorkbook workbook;

            var file = new FileStream(excelInfo.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            if (excelInfo.FileType.Equals("xlsx"))
            {
                workbook = new XSSFWorkbook(file);
            }
            else
            {
                workbook = new HSSFWorkbook(file);
            }
            file.Close();

            return workbook;

        }
        catch (Exception e)
        {
            Logger.Error($"{excelInfo.FilePath} {e.Message}");
        }
        return null;
    }

    private static List<SheetInfo>? GetSheet(IWorkbook workbook, string filePath)
    {
        List<SheetInfo> result = new();

        for (int i = 0; i < workbook.NumberOfSheets; ++i)
        {
            try
            {
                var sheet = workbook.GetSheetAt(i);

                KeyValuePair<string, List<FieldInfo>>? fieldInfos = GetField(sheet, filePath);

                if (fieldInfos == null)
                {
                    return null;
                }

                SheetInfo sheetInfo = new()
                {
                    FullName = sheet.SheetName,
                    FieldInfos = fieldInfos.Value.Value,
                    FkType =  fieldInfos.Value.Key,
                    Data = GetData(sheet)
                };

                string[] sheetNames = sheet.SheetName.Split("@");

                sheetInfo.Name = sheetNames[0];

                for (int j = 1; j < sheetNames.Length; ++j)
                {
                    sheetInfo.Hierarchy.Add(sheetNames[j]);
                }
                result.Add(sheetInfo);
            }
            catch (Exception e)
            {
                Logger.Error($"{filePath} {e.Message}");
            }
        }

        return result;
    }

    private static List<IRow> GetData(ISheet sheet)
    {
        List<IRow> result = new();

        for (int i = 0; i < sheet.LastRowNum - (DataStartIndex + 1); ++i)
        {
            result.Add(sheet.GetRow(i));
        }
        return result;
    }

    private static KeyValuePair<string, List<FieldInfo>>? GetField(ISheet sheet, string filePath)
    {
        List<FieldInfo> result = new();
        string fk = FieldType.ForeignKey;
        List<string> foreignKeyList = new()
        {
            FieldType.ForeignKey,
            FieldType.ForeignKeyList,
            FieldType.ForeignKeyDictionary,
        };

        var typeRow = sheet.GetRow(TypeIndex);
        var nameRow = sheet.GetRow(NameIndex);
        var commentRow = sheet.GetRow(CommentIndex);

        if (typeRow.Cells.Count <= 0)
        {
            Logger.Error($"{filePath} Sheet: {sheet.SheetName} type definition is null");
            return null;
        }

        if (typeRow.Cells.Count != nameRow.Cells.Count)
        {
            Logger.Error($"{filePath} Sheet: {sheet.SheetName} Type: {typeRow.Cells.Count} Name: {nameRow.Cells.Count} Field name and type quantity are inconsistent");
            return null;
        }

        for (int i = 0; i < typeRow.Cells.Count; ++i)
        {
            try
            {
                string? type = typeRow.GetCell(i).ToString();
                string? name = nameRow.GetCell(i).ToString();
                string comment = commentRow.GetCell(i).ToString()?.Replace("\n", "") ?? "";
                if (type == null || !new List<string>
                    {
                        FieldType.PrimaryKey,
                        FieldType.ForeignKey,
                        FieldType.ForeignKeyList,
                        FieldType.ForeignKeyDictionary,
                        FieldType.String,
                        FieldType.Int,
                        FieldType.Double,
                        FieldType.Bool,
                        FieldType.ListString,
                        FieldType.ListInt,
                        FieldType.ListDouble,
                        FieldType.ListBool
                    }.Contains(type))
                {
                    Logger.Error($"{filePath} Sheet: {sheet.SheetName} Cell: {i} Type: {type} field type is error");
                    return null;
                }
                if (name is null or "")
                {
                    if (type.Equals(FieldType.PrimaryKey))
                    {
                        name = "";
                    }
                    else
                    {
                        Logger.Error($"{filePath} Sheet: {sheet.SheetName} Cell {i} field name is error");
                        return null;
                    }
                }

                foreach (FieldInfo fi in result)
                {
                    if (name == fi.Name)
                    {
                        Logger.Error($"{filePath} Sheet: {sheet.SheetName} Cell {i} field names cannot be the same");
                        return null;
                    }

                    if (type.Equals(FieldType.PrimaryKey) && fi.Type.Equals(FieldType.PrimaryKey))
                    {
                        Logger.Error($"{filePath} Sheet: {sheet.SheetName} A Sheet can only contain one primary key");
                        return null;
                    }

                    if (!foreignKeyList.Contains(type) || !foreignKeyList.Contains(fi.Type))
                        continue;
                    Logger.Error($"{filePath} Sheet: {sheet.SheetName} A Sheet can only contain one foreign key");
                    return null;
                }

                FieldInfo fieldInfo = new()
                {
                    Index = i,
                    Name = name,
                    Type = type,
                    Comment = comment
                };
                result.Add(fieldInfo);
                if (foreignKeyList.Contains(type))
                    fk = type;
            }
            catch (Exception e)
            {
                Logger.Error($"{filePath} Cell {i} {e.Message}");
            }
        }

        if (result.Count != 1 || (!result[0].Type.Equals(FieldType.ForeignKey) && !result[0].Type.Equals(FieldType.ForeignKeyList) && !result[0].Type.Equals(FieldType.ForeignKeyDictionary)))
            return KeyValuePair.Create(fk, result);

        Logger.Error($"{filePath} Sheet: {sheet.SheetName} Contains at least one field other than fk or fks");
        return null;
    }
}
