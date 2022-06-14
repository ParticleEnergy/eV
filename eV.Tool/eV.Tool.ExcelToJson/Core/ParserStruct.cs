// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.EasyLog;
using eV.Tool.ExcelToJson.Define;
using eV.Tool.ExcelToJson.Model;
using Microsoft.Extensions.Configuration;
using FieldInfo = eV.Tool.ExcelToJson.Model.FieldInfo;
namespace eV.Tool.ExcelToJson.Core;

public class ParserStruct
{
    private readonly List<ObjectInfo> _objectInfos = new();
    private readonly IConfigurationRoot _configuration;
    public ParserStruct(IConfigurationRoot configuration)
    {
        _configuration = configuration;
    }

    public void OutClass(List<TableInfo> tableInfos, Action<string, string> write)
    {
        if (!CheckTable(tableInfos))
            return;

        _objectInfos.Clear();

        string path = _configuration.GetSection(Const.OutObjectFilePath).Value;

        foreach (TableInfo tableInfo in tableInfos)
        {
            ParserTableStruct(tableInfo);
        }

        foreach (ObjectInfo objectInfo in _objectInfos)
        {
            string file = objectInfo.IsMain ? $"{path}/{string.Format(Template.ObjectFileName, objectInfo.ClassName)}" : $"{path}/Object/{objectInfo.ClassName}.cs";
            write(file, objectInfo.ToString());
        }
    }

    private static string GetFiledType(string type)
    {
        return type switch
        {
            FieldType.ListString => "List<string>",
            FieldType.ListBool => "List<bool>",
            FieldType.ListDouble => "List<double>",
            FieldType.ListInt => "List<int>",
            FieldType.String => "string",
            FieldType.Bool => "bool",
            FieldType.Int => "int",
            FieldType.Double => "double",
            _ => "string"
        };
    }

    private static string GetDefaultValue(string type)
    {
        return type switch
        {
            FieldType.String => "string.Empty",
            FieldType.Bool => "false",
            FieldType.Double => "0.0",
            FieldType.Int => "0",
            _ => "string.Empty"
        };
    }

    private ObjectInfo CreateObjectInfo()
    {
        string outObjectNamespace = _configuration.GetSection(Const.OutObjectNamespace).Value;
        string outObjectFileHead = _configuration.GetSection(Const.OutObjectFileHead).Value;
        return new ObjectInfo
        {
            NamespaceName = outObjectNamespace, Head = outObjectFileHead
        };
    }

    private ObjectInfo GetObjectInfo(SheetInfo sheetInfo)
    {
        ObjectInfo objectInfo = CreateObjectInfo();

        foreach (var fieldInfo in sheetInfo.FieldInfos.Where(fieldInfo => !FieldType.ForeignKeyTypes.Contains(fieldInfo.Type)))
        {
            if (FieldType.ListTypes.Contains(fieldInfo.Type))
            {
                objectInfo.ObjectComplexProperties.Add(new ObjectComplexProperty
                {
                    Type = GetFiledType(fieldInfo.Type),
                    Name = fieldInfo.Name,
                    Comment = fieldInfo.Comment
                });
            }
            else if (FieldType.PrimaryKeyTypes.Contains(fieldInfo.Type))
            {
                objectInfo.ObjectBaseProperties.Add(new ObjectBaseProperty
                {
                    Type = GetFiledType(FieldType.String),
                    Name = fieldInfo.Name,
                    DefaultValue = GetDefaultValue(FieldType.String),
                    Comment = fieldInfo.Comment
                });
            }
            else
            {
                objectInfo.ObjectBaseProperties.Add(new ObjectBaseProperty
                {
                    Type = GetFiledType(fieldInfo.Type),
                    Name = fieldInfo.Name,
                    DefaultValue = GetDefaultValue(fieldInfo.Type),
                    Comment = fieldInfo.Comment
                });
            }
        }
        return objectInfo;
    }

    private void ParserTableStruct(TableInfo tableInfo)
    {
        Dictionary<string, ObjectInfo> objectInfos = new()
        {
            [Const.MainSheet] = GetObjectInfo(tableInfo.MainSheet)
        };

        objectInfos[Const.MainSheet].IsMain = true;
        objectInfos[Const.MainSheet].ClassName = tableInfo.FileName;
        objectInfos[Const.MainSheet].ProfileType = tableInfo.MainSheet.PrimaryKeyFieldInfo!.Type == FieldType.PrimaryKeyList ? "List" : "Dictionary";
        objectInfos[Const.MainSheet].ProfileDetailType = tableInfo.MainSheet.PrimaryKeyFieldInfo!.Type == FieldType.PrimaryKeyList ? "" : "string, ";

        foreach (SheetInfo sheetInfo in tableInfo.SubSheetInfos)
        {
            var objectInfo = GetObjectInfo(sheetInfo);
            objectInfo.IsMain = false;
            objectInfo.ClassName = sheetInfo.Name;

            string key = sheetInfo.Hierarchy.Count == 0 ? Const.MainSheet : sheetInfo.Hierarchy[0];

            switch (sheetInfo.ForeignKeyFieldInfo!.Type)
            {
                case FieldType.ForeignKeyDictionary:
                    objectInfos[key].ObjectComplexProperties.Add(new ObjectComplexProperty
                    {
                        Name = $"{sheetInfo.Name}Dictionary",
                        Type = $"Dictionary<string, {sheetInfo.Name}>",
                        Comment = sheetInfo.Name
                    });
                    break;
                case FieldType.ForeignKeyList:
                    objectInfos[key].ObjectComplexProperties.Add(new ObjectComplexProperty
                    {
                        Type = $"List<{sheetInfo.Name}>",
                        Name = $"{sheetInfo.Name}List",
                        Comment = sheetInfo.Name
                    });
                    break;
                default:
                    objectInfos[key].ObjectComplexProperties.Add(new ObjectComplexProperty
                    {
                        Type = sheetInfo.Name,
                        Name = sheetInfo.Name,
                        Comment = sheetInfo.Name
                    });
                    break;
            }

            objectInfos[sheetInfo.Name] = objectInfo;
        }

        _objectInfos.Add(objectInfos[Const.MainSheet]);

        foreach ((string _, ObjectInfo oi) in objectInfos)
        {
            bool flag = false;
            foreach (var _ in _objectInfos.Where(goi => oi.ClassName == goi.ClassName))
            {
                flag = true;
            }
            if (flag)
                continue;
            _objectInfos.Add(oi);
        }
    }

    private static bool CheckTable(IEnumerable<TableInfo> tableInfos)
    {
        List<KeyValuePair<string, SheetInfo>> allSheetInfos = new();
        foreach (TableInfo tableInfo in tableInfos)
        {
            allSheetInfos.AddRange(tableInfo.SubSheetInfos.Select(sheetInfo => KeyValuePair.Create(tableInfo.FilePath, sheetInfo)));
        }


        foreach ((string filePath1, SheetInfo sheetInfo1) in allSheetInfos)
        {
            foreach ((string filePath2, SheetInfo sheetInfo2) in allSheetInfos)
            {
                if (!sheetInfo1.Name.Equals(sheetInfo2.Name))
                    continue;

                if (sheetInfo1.FieldInfos.Count != sheetInfo2.FieldInfos.Count)
                {
                    Logger.Error($"[{filePath1} Sheet: {sheetInfo1.FullName}] [{filePath2} Sheet: {sheetInfo2.FullName}] Unequal number of fields");
                    return false;
                }

                foreach (FieldInfo fieldInfo1 in sheetInfo1.FieldInfos)
                {
                    if (FieldType.ForeignKeyTypes.Contains(fieldInfo1.Type))
                        continue;
                    bool flag = false;
                    foreach (var _ in sheetInfo2.FieldInfos.Where(fieldInfo2 => fieldInfo1.Name.Equals(fieldInfo2.Name) && fieldInfo2.Type.Equals(fieldInfo2.Type)))
                    {
                        flag = true;
                    }
                    if (flag)
                        continue;
                    Logger.Error($"[{filePath1} Sheet: {sheetInfo1.FullName}] [{filePath2} Sheet: {sheetInfo2.FullName}] Inconsistent fields [{fieldInfo1.Name}]");
                    return false;
                }
            }
        }
        return true;
    }
}
