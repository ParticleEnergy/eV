// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.EasyLog;
using eV.Tool.ExcelToJson.Define;
using eV.Tool.ExcelToJson.Excel;
using eV.Tool.ExcelToJson.Model;
using Microsoft.Extensions.Configuration;
namespace eV.Tool.ExcelToJson.Core;

public class AnalyticStructure
{
    public Action<string, string>? Write { get; set; }
    private readonly IConfigurationRoot _configuration;
    private readonly List<ExcelInfo> _excelInfos;
    private readonly List<ObjectInfo> _objectInfos = new();
    public AnalyticStructure(IConfigurationRoot configuration, List<ExcelInfo> excelInfos)
    {
        _configuration = configuration;
        _excelInfos = excelInfos;
    }

    public void Generate()
    {
        if (!CheckTable())
            return;

        _objectInfos.Clear();
        string path = _configuration.GetSection(Const.OutObjectFilePath).Value;
        foreach (ExcelInfo excel in _excelInfos)
        {
            Analytic(excel);
        }
        foreach (ObjectInfo objectInfo in _objectInfos)
        {
            string file = objectInfo.IsMain ? $"{path}/{string.Format(Template.ObjectFileName, objectInfo.ClassName)}" : $"{path}/Object/{objectInfo.ClassName}.cs";
            Write?.Invoke(file, objectInfo.ToString());
        }
    }

    private string GetFiledType(string type)
    {
        return type switch
        {

            FieldType.String => "string",
            FieldType.Bool => "bool",
            FieldType.Int => "int",
            FieldType.Double => "double",
            _ => string.Join(", ", type.Replace(" ", "").Split(Const.SplitFlag)).ToLower().Replace(FieldType.Dict.ToLower(), "Dictionary").Replace(FieldType.List.ToLower(), "List")
        };
    }

    private string GetDefaultValue(string type)
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
        return new ObjectInfo(_configuration.GetSection(Const.CSharpVersion).Value)
        {
            NamespaceName = outObjectNamespace,
            Head = outObjectFileHead,
            IsDependencies = true
        };
    }

    private ObjectInfo GetObjectInfo(SheetInfo sheetInfo)
    {
        ObjectInfo objectInfo = CreateObjectInfo();

        if (sheetInfo.PrimaryKeyFieldInfo != null)
        {
            objectInfo.ObjectBaseProperties.Add(new ObjectBaseProperty
            {
                Type = GetFiledType(FieldType.String),
                Name = sheetInfo.PrimaryKeyFieldInfo.Name,
                DefaultValue = GetDefaultValue(FieldType.String),
                Comment = sheetInfo.PrimaryKeyFieldInfo.Comment
            });
        }

        foreach (var fieldInfo in sheetInfo.FieldInfos.Where(fieldInfo => !FieldType.ForeignKeyTypes.Contains(fieldInfo.Type)))
        {
            if (fieldInfo.Type.StartsWith(FieldType.Dict) || fieldInfo.Type.StartsWith(FieldType.List))
            {
                objectInfo.ObjectComplexProperties.Add(new ObjectComplexProperty
                {
                    Type = GetFiledType(fieldInfo.Type),
                    Name = fieldInfo.Name,
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

    private void Analytic(ExcelInfo excelInfo)
    {
        if (excelInfo.MainSheetInfo == null)
            return;

        Dictionary<string, ObjectInfo> objectInfos = new()
        {
            [Const.MainSheet] = GetObjectInfo(excelInfo.MainSheetInfo)
        };
        objectInfos[Const.MainSheet].IsMain = true;
        objectInfos[Const.MainSheet].ClassName = excelInfo.FileName;
        objectInfos[Const.MainSheet].ProfileType = excelInfo.MainSheetInfo.PrimaryKeyFieldInfo!.Type == FieldType.PrimaryKeyList ? "List" : "Dictionary";
        objectInfos[Const.MainSheet].ProfileDetailType = excelInfo.MainSheetInfo.PrimaryKeyFieldInfo!.Type == FieldType.PrimaryKeyList ? "" : "string, ";
        objectInfos[Const.MainSheet].IsDependencies = excelInfo.SubSheetInfos.Count > 0;

        foreach (SheetInfo sheetInfo in excelInfo.SubSheetInfos)
        {
            if (sheetInfo.PrimaryKeyFieldInfo == null || sheetInfo.ForeignKeyFieldInfo == null)
                continue;

            var objectInfo = GetObjectInfo(sheetInfo);
            objectInfo.IsMain = false;
            objectInfo.ClassName = sheetInfo.Name;

            string key = sheetInfo.Hierarchy.Count == 0 ? Const.MainSheet : sheetInfo.Hierarchy[0];

            switch (sheetInfo.ForeignKeyFieldInfo.Type)
            {
                case FieldType.ForeignKeyDictionary:
                    objectInfos[key].ObjectComplexProperties.Add(new ObjectComplexProperty
                    {
                        Type = $"Dictionary<string, {sheetInfo.Name}>",
                        Name = $"{sheetInfo.Name}Dictionary",
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
            foreach (var _ in _objectInfos.Where(alreadyExistObjectInfo => oi.ClassName == alreadyExistObjectInfo.ClassName))
                flag = true;

            if (flag)
                continue;
            _objectInfos.Add(oi);
        }
    }

    private bool CheckTable()
    {
        List<KeyValuePair<string, SheetInfo>> allSheetInfos = new();
        foreach (ExcelInfo excelInfo in _excelInfos)
        {
            allSheetInfos.AddRange(excelInfo.SubSheetInfos.Select(sheetInfo => KeyValuePair.Create(excelInfo.FilePath, sheetInfo)));
        }

        foreach ((string filePath1, SheetInfo sheetInfo1) in allSheetInfos)
        {
            foreach ((string filePath2, SheetInfo sheetInfo2) in allSheetInfos)
            {
                if (!sheetInfo1.Name.Equals(sheetInfo2.Name))
                    continue;

                if (sheetInfo1.FieldInfos.Count != sheetInfo2.FieldInfos.Count)
                {
                    Logger.Error($"[{filePath1} Sheet: {sheetInfo1.FullName}] [{filePath2} Sheet: {sheetInfo2.FullName}] unequal number of fields");
                    return false;
                }

                foreach (FieldInfo fieldInfo1 in sheetInfo1.FieldInfos)
                {
                    // check fk
                    if (FieldType.ForeignKeyTypes.Contains(fieldInfo1.Type))
                        continue;

                    bool flag = false;

                    foreach (var _ in sheetInfo2.FieldInfos.Where(fieldInfo2 => fieldInfo1.Name.Equals(fieldInfo2.Name) && fieldInfo2.Type.Equals(fieldInfo2.Type)))
                        flag = true;

                    if (flag)
                        continue;
                    Logger.Error($"[{filePath1} Sheet: {sheetInfo1.FullName}] [{filePath2} Sheet: {sheetInfo2.FullName}] inconsistent fields [{fieldInfo1.Name}]");
                    return false;
                }
            }
        }
        return true;
    }
}
