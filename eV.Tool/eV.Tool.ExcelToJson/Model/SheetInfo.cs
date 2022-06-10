// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Tool.ExcelToJson.Define;
using NPOI.SS.UserModel;
namespace eV.Tool.ExcelToJson.Model;

public class SheetInfo : IComparable<SheetInfo>
{
    public string FullName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<string> Hierarchy { get; set; } = new();

    public string FkType { get; set; } = string.Empty;
    public List<FieldInfo>? FieldInfos { get; set; }
    public List<IRow>? Data { get; set; }

    public int CompareTo(SheetInfo? other)
    {
        if (Name == Template.ProfileName)
            return 1;

        if (other?.Name == Template.ProfileName)
            return -1;

        int flag = other?.Hierarchy.Count ?? 0;

        return flag > Hierarchy.Count ? 1 : -1;
    }
}
