// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.EasyLog;
using eV.Tool.ExcelToJson.Define;
using eV.Tool.ExcelToJson.Model;
namespace eV.Tool.ExcelToJson.Core;

public class Parser
{
    private List<ObjectInfo> _objectInfos;

    public Parser()
    {

    }

    private void ParserTableStruct(TableInfo tableInfo)
    {
        Dictionary<string, ObjectInfo> objectInfos = new();

        foreach (var sheetInfo in tableInfo.SheetInfos!.Where(sheetInfo => _objectInfos.All(oi => oi.ClassName != sheetInfo.Name)))
        {
            ObjectInfo objectInfo = new();
            if (sheetInfo.Name == Template.ProfileName)
            {
                objectInfo.IsMain = true;
                objectInfo.ClassName = tableInfo.FileName;
            }
            else
            {
                objectInfo.IsMain = false;
                objectInfo.ClassName = sheetInfo.Name;
            }

            objectInfos[sheetInfo.Name] = objectInfo;
        }

        //

        //
        // _objectInfos.Add(objectInfo);
    }

    private static bool CheckTable(List<TableInfo> tableInfos)
    {
        List<KeyValuePair<string, SheetInfo>> allSheetInfos = (from tableInfo in tableInfos from sheetInfo in tableInfo.SheetInfos! where !sheetInfo.Name.Equals(Template.ProfileName) select KeyValuePair.Create(tableInfo.FilePath, sheetInfo)).ToList();


        foreach ((string filePath1, SheetInfo sheetInfo1) in allSheetInfos)
        {
            foreach ((string filePath2, SheetInfo sheetInfo2) in allSheetInfos)
            {
                if (!sheetInfo1.Name.Equals(sheetInfo2.Name))
                    continue;

                if (sheetInfo1.FieldInfos!.Count != sheetInfo2.FieldInfos!.Count)
                {
                    Logger.Error($"[{filePath1} Sheet: {sheetInfo1.FullName}] [{filePath2} Sheet: {sheetInfo2.FullName}] Unequal number of fields");
                    return false;
                }

                foreach (FieldInfo fieldInfo1 in sheetInfo1.FieldInfos)
                {
                    if (fieldInfo1.Type.Equals(FieldType.ForeignKey) || fieldInfo1.Type.Equals(FieldType.ForeignKeyList) || fieldInfo1.Type.Equals(FieldType.ForeignKeyDictionary))
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
