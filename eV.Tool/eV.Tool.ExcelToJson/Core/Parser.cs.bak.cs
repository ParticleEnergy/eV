// // Copyright (c) ParticleEnergy. All rights reserved.
// // Licensed under the Apache license. See the LICENSE file in the project root for full license information.
//
// using eV.Module.EasyLog;
// using eV.Tool.ExcelToJson.Define;
// using eV.Tool.ExcelToJson.Model;
// namespace eV.Tool.ExcelToJson.Core;
//
// public class Parser
// {
//     private readonly List<ObjectInfo> _objectInfos = new();
//
//     public Parser(IEnumerable<TableInfo> tableInfos)
//     {
//         CheckTable(tableInfos);
//     }
//
//     private string GetListType(string type)
//     {
//         return type switch
//         {
//             FieldType.ListString => "List<string>",
//             FieldType.ListBool => "List<bool>",
//             FieldType.ListDouble => "List<double>",
//             FieldType.ListInt => "List<int>",
//             _ => "string.Empty"
//         };
//     }
//
//     private string GetDefaultValue(string type)
//     {
//         return type switch
//         {
//             FieldType.String => "string.Empty",
//             FieldType.Bool => "false",
//             FieldType.Double => "0.0",
//             FieldType.Int => "0",
//             _ => "string.Empty"
//         };
//     }
//
//     private ObjectInfo GetObjectInfo(SheetInfo sheetInfo)
//     {
//         ObjectInfo objectInfo = new()
//         {
//             Head = "//",
//             NamespaceName = "eV",
//         };
//
//
//         foreach (var fieldInfo in sheetInfo.FieldInfos.Where(fieldInfo => !fieldInfo.Type.Equals(FieldType.PrimaryKey) || !fieldInfo.Name.Equals("")))
//         {
//             if (fieldInfo.Type.Equals(FieldType.PrimaryKey))
//             {
//                 objectInfo.ObjectBaseProperties.Add(new ObjectBaseProperty
//                 {
//                     Type = fieldInfo.Type,
//                     Name = fieldInfo.Name,
//                     DefaultValue = GetDefaultValue(FieldType.String)
//                 });
//             }
//             else if (new List<string>{FieldType.ListBool,FieldType.ListInt,FieldType.ListDouble,FieldType.ListString}.Contains(fieldInfo.Type))
//             {
//                 objectInfo.ObjectComplexProperties.Add(new ObjectComplexProperty
//                 {
//                     Type = GetListType(fieldInfo.Type),
//                     Name = fieldInfo.Name
//                 });
//             }
//             else
//             {
//                 objectInfo.ObjectBaseProperties.Add(new ObjectBaseProperty
//                 {
//                     Type = fieldInfo.Type,
//                     Name = fieldInfo.Name,
//                     DefaultValue = GetDefaultValue(fieldInfo.Type)
//                 });
//             }
//         }
//
//
//         return objectInfo;
//     }
//
//     private void ParserTableStruct(TableInfo tableInfo)
//     {
//         Dictionary<string, ObjectInfo> objectInfos = new();
//
//         foreach (var sheetInfo in tableInfo.SheetInfos!.Where(sheetInfo => _objectInfos.All(oi => oi.ClassName != sheetInfo.Name)))
//         {
//             ObjectInfo objectInfo;
//             if (sheetInfo.Name == Template.ProfileName)
//             {
//                 objectInfo = GetObjectInfo(sheetInfo);
//                 objectInfo.ClassName = tableInfo.FileName;
//                 objectInfo.IsMain = true;
//                 objectInfo.ProfileType = sheetInfo.PkType;
//                 objectInfo.ProfileDetailType = sheetInfo.PkType == "List" ? "": "string, ";
//             }
//             else
//             {
//                 objectInfo = GetObjectInfo(sheetInfo);
//                 objectInfo.ClassName = sheetInfo.Name;
//                 objectInfo.IsMain = false;
//
//                 string key = sheetInfo.Hierarchy.Count == 0? Template.ProfileName : sheetInfo.Hierarchy[0];
//                 if (sheetInfo.FkType.Equals(FieldType.ForeignKey))
//                 {
//                     objectInfos[key].ObjectComplexProperties.Add(new ObjectComplexProperty
//                     {
//                         Name = objectInfo.ClassName,
//                         Type = objectInfo.ClassName
//                     });
//                 }
//                 else if (sheetInfo.FkType.Equals(FieldType.ForeignKeyList))
//                 {
//                     objectInfos[key].ObjectComplexProperties.Add(new ObjectComplexProperty
//                     {
//                         Name = $"{objectInfo.ClassName}List",
//                         Type = $"List<{objectInfo.ClassName}>"
//                     });
//                 }
//                 else
//                 {
//                     objectInfos[key].ObjectComplexProperties.Add(new ObjectComplexProperty
//                     {
//                         Name = $"{objectInfo.ClassName}Dictionary",
//                         Type = $"Dictionary<string, {objectInfo.ClassName}>"
//                     });
//                 }
//             }
//
//             objectInfos[sheetInfo.Name] = objectInfo;
//         }
//     }
//
//     private static bool CheckTable(IEnumerable<TableInfo> tableInfos)
//     {
//         List<KeyValuePair<string, SheetInfo>> allSheetInfos = (from tableInfo in tableInfos from sheetInfo in tableInfo.SheetInfos! where !sheetInfo.Name.Equals(Template.ProfileName) select KeyValuePair.Create(tableInfo.FilePath, sheetInfo)).ToList();
//
//
//         foreach ((string filePath1, SheetInfo sheetInfo1) in allSheetInfos)
//         {
//             foreach ((string filePath2, SheetInfo sheetInfo2) in allSheetInfos)
//             {
//                 if (!sheetInfo1.Name.Equals(sheetInfo2.Name))
//                     continue;
//
//                 if (sheetInfo1.FieldInfos.Count != sheetInfo2.FieldInfos.Count)
//                 {
//                     Logger.Error($"[{filePath1} Sheet: {sheetInfo1.FullName}] [{filePath2} Sheet: {sheetInfo2.FullName}] Unequal number of fields");
//                     return false;
//                 }
//
//                 foreach (FieldInfo fieldInfo1 in sheetInfo1.FieldInfos)
//                 {
//                     if (fieldInfo1.Type.Equals(FieldType.ForeignKey) || fieldInfo1.Type.Equals(FieldType.ForeignKeyList) || fieldInfo1.Type.Equals(FieldType.ForeignKeyDictionary))
//                         continue;
//                     bool flag = false;
//                     foreach (var _ in sheetInfo2.FieldInfos.Where(fieldInfo2 => fieldInfo1.Name.Equals(fieldInfo2.Name) && fieldInfo2.Type.Equals(fieldInfo2.Type)))
//                     {
//                         flag = true;
//                     }
//                     if (flag)
//                         continue;
//                     Logger.Error($"[{filePath1} Sheet: {sheetInfo1.FullName}] [{filePath2} Sheet: {sheetInfo2.FullName}] Inconsistent fields [{fieldInfo1.Name}]");
//                     return false;
//                 }
//             }
//         }
//         return true;
//     }
// }
