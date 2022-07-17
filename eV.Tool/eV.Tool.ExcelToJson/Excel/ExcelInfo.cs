// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.EasyLog;
using eV.Tool.ExcelToJson.Define;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
namespace eV.Tool.ExcelToJson.Excel;

public class ExcelInfo
{
    public string FilePath { get; }
    public string FileName { get; }
    public string FileType { get; }
    public SheetInfo? MainSheetInfo { get; private set; }
    public List<SheetInfo> SubSheetInfos { get; } = new();

    public ExcelInfo(string filePath, string fileName, string fileType)
    {
        FilePath = filePath;
        FileName = fileName;
        FileType = fileType;

        ParserSheet();
    }

    private IWorkbook? GetWorkbook()
    {
        try
        {
            var file = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            IWorkbook workbook = FileType.Equals("xlsx") ? new XSSFWorkbook(file) : new HSSFWorkbook(file);
            file.Close();
            return workbook;
        }
        catch (Exception e)
        {
            Logger.Error($"{FilePath} {e.Message}");
        }
        return null;
    }

    private void ParserSheet()
    {
        var workbook = GetWorkbook();
        if (workbook == null)
        {
            Logger.Error($"{FilePath} workbook is null");
            return;
        }
        for (int i = 0; i < workbook.NumberOfSheets; ++i)
        {
            var sheet = workbook.GetSheetAt(i);
            if (sheet == null || sheet.SheetName.StartsWith(Const.IgnoreFlag))
                continue;

            var sheetInfo = GetSheetInfo(sheet);
            if (sheetInfo == null)
                continue;

            string[] sheetNames = sheet.SheetName.Split("@");
            sheetInfo.Name = sheetNames[0];
            for (int j = 1; j < sheetNames.Length; j++)
            {
                sheetInfo.Hierarchy.Add(sheetNames[j]);
            }

            if (sheetInfo.Name == Const.MainSheet)
            {
                MainSheetInfo = sheetInfo;
            }
            else
            {
                SubSheetInfos.Add(sheetInfo);
            }
        }
        if (MainSheetInfo == null)
        {
            Logger.Error($"{FilePath} main sheet not found");
        }
    }

    private SheetInfo? GetSheetInfo(ISheet sheet)
    {
        SheetInfo sheetInfo = new()
        {
            FullName = sheet.SheetName
        };

        var commentRow = sheet.GetRow(Const.CommentRowIndex);
        var nameRow = sheet.GetRow(Const.NameRowIndex);
        var typeRow = sheet.GetRow(Const.TypeRowIndex);

        if (commentRow == null)
        {
            Logger.Error($"{FilePath} Sheet: {sheet.SheetName} comment row not found");
            return null;
        }
        if (nameRow == null)
        {
            Logger.Error($"{FilePath} Sheet: {sheet.SheetName} name row not found");
            return null;
        }
        if (typeRow == null)
        {
            Logger.Error($"{FilePath} Sheet: {sheet.SheetName} type row not found");
            return null;
        }

        for (int i = 0; i < nameRow.Cells.Count; i++)
        {
            string comment = commentRow.GetCell(i)?.ToString()?.Replace("\n", "") ?? "";
            string name = nameRow.GetCell(i)?.ToString() ?? "";
            string type = typeRow.GetCell(i)?.ToString() ?? "";

            // check null
            if (name.Equals(""))
            {
                Logger.Error($"{FilePath} Sheet: {sheet.SheetName} Cell: {i + 1} name is empty");
                continue;
            }
            if (comment.Equals(""))
            {
                Logger.Error($"{FilePath} Sheet: {sheet.SheetName} Cell: {i + 1} comment is empty");
                continue;
            }
            if (type.Equals(""))
            {
                Logger.Error($"{FilePath} Sheet: {sheet.SheetName} Cell: {i + 1} type is empty");
                continue;
            }

            if (name.StartsWith(Const.IgnoreFlag))
                continue;

            var fieldInfo = GetFieldInfo(comment, name, type, i, sheet.SheetName);
            if (fieldInfo == null)
                continue;

            foreach (FieldInfo alreadyExistFieldInfo in sheetInfo.FieldInfos)
            {
                if (fieldInfo.Name == alreadyExistFieldInfo.Name)
                {
                    Logger.Error($"{FilePath} Sheet: {sheet.SheetName} Cell: {i + 1} field names cannot be the same");
                    return null;
                }

                if (FieldType.PrimaryKeyTypes.Contains(fieldInfo.Type) && FieldType.PrimaryKeyTypes.Contains(alreadyExistFieldInfo.Type))
                {
                    Logger.Error($"{FilePath} Sheet: {sheet.SheetName} Cell: {i + 1} a sheet can only contain one primary key");
                    return null;
                }

                if (!FieldType.ForeignKeyTypes.Contains(fieldInfo.Type) || !FieldType.ForeignKeyTypes.Contains(alreadyExistFieldInfo.Type))
                    continue;
                Logger.Error($"{FilePath} Sheet: {sheet.SheetName} Cell: {i + 1} a sheet can only contain one foreign key");
                return null;
            }

            if (FieldType.PrimaryKeyTypes.Contains(type))
            {
                sheetInfo.PrimaryKeyFieldInfo = fieldInfo;
            }
            else if (FieldType.ForeignKeyTypes.Contains(type))
            {
                sheetInfo.ForeignKeyFieldInfo = fieldInfo;
            }
            else
            {
                sheetInfo.FieldInfos.Add(fieldInfo);
            }
        }

        // check pk fk
        if (sheetInfo.PrimaryKeyFieldInfo == null)
        {
            Logger.Error($"{FilePath} Sheet: {sheet.SheetName} must have primary key");
            return null;
        }
        if (!sheet.SheetName.Equals(Const.MainSheet) && sheetInfo.ForeignKeyFieldInfo == null)
        {
            Logger.Error($"{FilePath} Sheet: {sheet.SheetName} non main sheet must contain foreign keys");
            return null;
        }
        if (sheetInfo.ForeignKeyFieldInfo == null && sheetInfo.PrimaryKeyFieldInfo == null && sheetInfo.FieldInfos.Count == 0)
        {
            Logger.Error($"{FilePath} Sheet: {sheet.SheetName} contains at least one field");
            return null;
        }

        // data
        for (int i = Const.DataStartRowIndex; i <= sheet.LastRowNum; ++i)
        {
            var row = sheet.GetRow(i);
            if (row is not { LastCellNum: > 0 })
            {
                Logger.Error($"{FilePath} {sheet.SheetName} Row :{i + 1} is null");
                continue;
            }
            string pk = row.GetCell(sheetInfo.PrimaryKeyFieldInfo!.Index)?.ToString() ?? "";

            if (pk.Equals(""))
                continue;

            sheetInfo.Data.Add(sheet.GetRow(i));
        }

        return sheetInfo;
    }

    private FieldInfo? GetFieldInfo(string comment, string name, string type, int index, string sheetName)
    {
        // check type
        if (!FieldType.PrimaryKeyTypes.Contains(type)
            && !FieldType.ForeignKeyTypes.Contains(type)
            && !FieldType.BaseTypes.Contains(type)
            && !FieldType.ListTypes.Contains(type)
            && !type.StartsWith(FieldType.Dict)
            && !type.Equals(FieldType.Class))
        {
            Logger.Error($"{FilePath} Sheet: {sheetName} Cell: {index + 1} type definition error");
            return null;
        }

        if (!sheetName.Equals(Const.MainSheet) && FieldType.PrimaryKey.Contains(type) && !FieldType.PrimaryKey.Equals(type))
        {
            Logger.Error($"{FilePath} Sheet: {sheetName} Cell: {index + 1} non main sheet primary keys are defined as pk");
            return null;
        }

        if (sheetName.Equals(Const.MainSheet) && FieldType.ForeignKeyTypes.Contains(type))
        {
            Logger.Error($"{FilePath} Sheet: {sheetName} Cell: {index + 1} foreign key type cannot exist in main sheet");
            return null;
        }

        FieldInfo fieldInfo = new()
        {
            Index = index,
            Comment = comment,
            Name = name,
            Type = type
        };

        return fieldInfo;
    }
}
