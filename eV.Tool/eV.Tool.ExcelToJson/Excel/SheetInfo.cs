// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Tool.ExcelToJson.Define;
using NPOI.SS.UserModel;
namespace eV.Tool.ExcelToJson.Excel;

public class SheetInfo : IComparable<SheetInfo>
{
    public string FullName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<IRow> Data { get; } = new();
    public FieldInfo? PrimaryKeyFieldInfo { get; set; }
    public FieldInfo? ForeignKeyFieldInfo { get; set; }
    public List<FieldInfo> FieldInfos { get; } = new();
    public List<string> Hierarchy { get; } = new();
    public int CompareTo(SheetInfo? other)
    {
        if (Name == Const.MainSheet)
            return -1;

        if (other?.Name == Const.MainSheet)
            return 1;

        int flag = other?.Hierarchy.Count ?? 0;

        return flag > Hierarchy.Count ? -1 : 1;
    }
}
