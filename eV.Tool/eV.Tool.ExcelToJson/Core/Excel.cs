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
    public static List<TableInfo>? GetTableInfos(IEnumerable<ExcelInfo> excelInfos)
    {
        List<TableInfo> result = new();
        foreach (ExcelInfo excelInfo in excelInfos)
        {
            var workbook = GetWorkbook(excelInfo);
            if (workbook == null)
                return null;

            var tableInfo = GetTableInfo(workbook, excelInfo);
            if (tableInfo == null)
                return null;
            result.Add(tableInfo);
        }
        return result;
    }

    private static TableInfo? GetTableInfo(IWorkbook workbook, ExcelInfo excelInfo)
    {
        TableInfo tableInfo = new()
        {
            FileName = excelInfo.FileName, FilePath = excelInfo.FilePath
        };

        for (int i = 0; i < workbook.NumberOfSheets; ++i)
        {
            var sheet = workbook.GetSheetAt(i);

            if (sheet.SheetName.StartsWith("!"))
                continue;

            var sheetInfo = GetSheetInfo(sheet, excelInfo.FilePath);
            if (sheetInfo == null)
                return null;

            if (sheet.SheetName.Equals(Const.MainSheet))
            {
                tableInfo.MainSheet = sheetInfo;
                sheetInfo.Name = Const.MainSheet;
            }
            else
            {
                tableInfo.SubSheetInfos.Add(sheetInfo);

                string[] sheetNames = sheet.SheetName.Split("@");

                sheetInfo.Name = sheetNames[0];

                for (int j = 1; j < sheetNames.Length; ++j)
                {
                    sheetInfo.Hierarchy.Add(sheetNames[j]);
                }
            }
        }
        tableInfo.SubSheetInfos.Sort();
        return tableInfo;
    }

    private static SheetInfo? GetSheetInfo(ISheet sheet, string filePath)
    {
        SheetInfo sheetInfo = new()
        {
            FullName = sheet.SheetName
        };
        // field
        var commentRow = sheet.GetRow(Const.CommentRowIndex);
        var nameRow = sheet.GetRow(Const.NameRowIndex);
        var typeRow = sheet.GetRow(Const.TypeRowIndex);

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

        for (int i = 0; i < typeRow.Cells.Count; i++)
        {
            try
            {
                string comment = commentRow.GetCell(i).ToString()?.Replace("\n", "") ?? "";
                string? name = nameRow.GetCell(i).ToString();
                string? type = typeRow.GetCell(i).ToString();

                if (name != null && name.StartsWith("!"))
                    continue;

                if (type == null || !FieldType.AllTypes.Contains(type))
                {
                    Logger.Error($"{filePath} Sheet: {sheet.SheetName} Cell: {i} Type: {type} field type is error");
                    return null;
                }

                if (FieldType.PrimaryKeyTypes.Contains(type) && !sheet.SheetName.Equals(Const.MainSheet) && !type.Equals(FieldType.PrimaryKey))
                {
                    Logger.Error($"{filePath} Sheet: {sheet.SheetName} Cell: {i} Type: {type} primary key type is error");
                    return null;
                }

                if (name is null or "" && !FieldType.ForeignKeyTypes.Contains(type))
                {
                    Logger.Error($"{filePath} Sheet: {sheet.SheetName} Cell {i} field name is error");
                    return null;
                }

                foreach (FieldInfo fi in sheetInfo.FieldInfos)
                {
                    if (name == fi.Name)
                    {
                        Logger.Error($"{filePath} Sheet: {sheet.SheetName} Cell {i} field names cannot be the same");
                        return null;
                    }

                    if (FieldType.PrimaryKeyTypes.Contains(type) && FieldType.PrimaryKeyTypes.Contains(fi.Type))
                    {
                        Logger.Error($"{filePath} Sheet: {sheet.SheetName} A Sheet can only contain one primary key");
                        return null;
                    }

                    if (!FieldType.ForeignKeyTypes.Contains(type) || !FieldType.ForeignKeyTypes.Contains(fi.Type))
                        continue;
                    Logger.Error($"{filePath} Sheet: {sheet.SheetName} A Sheet can only contain one foreign key");
                    return null;
                }

                FieldInfo fieldInfo = new()
                {
                    Index = i,
                    Name = name ?? "",
                    Type = type,
                    Comment = comment
                };

                if (FieldType.ForeignKeyTypes.Contains(type))
                {
                    sheetInfo.ForeignKeyFieldInfo = fieldInfo;
                }
                else if (FieldType.PrimaryKeyTypes.Contains(type))
                {
                    sheetInfo.PrimaryKeyFieldInfo = fieldInfo;
                }
                else
                {
                    sheetInfo.FieldInfos.Add(fieldInfo);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"{filePath} Cell {i} {e.Message}");
            }
        }
        if (!sheet.SheetName.Equals(Const.MainSheet) && sheetInfo.ForeignKeyFieldInfo == null)
        {
            Logger.Error($"{filePath} Sheet: {sheet.SheetName} Non Main Sheet must contain foreign keys");
            return null;
        }
        if (sheetInfo.ForeignKeyFieldInfo == null && sheetInfo.PrimaryKeyFieldInfo == null && sheetInfo.FieldInfos.Count == 0)
        {
            Logger.Error($"{filePath} Sheet: {sheet.SheetName} Contains at least one field");
            return null;
        }
        if (sheet.SheetName.Equals(Const.MainSheet) && sheetInfo.PrimaryKeyFieldInfo == null)
        {
            Logger.Error($"{filePath} Sheet: {sheet.SheetName} Main sheet must have primary key");
            return null;
        }

        // data
        for (int i = Const.DataStartRowIndex; i <= sheet.LastRowNum; ++i)
        {
            if (sheet.GetRow(i) == null)
            {
                Logger.Error($"{filePath} {sheet.SheetName}row :{i} is null");
                return null;
            }
            sheetInfo.Data.Add(sheet.GetRow(i));
        }

        return sheetInfo;
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
}
