// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Tool.ExcelToJson.Model;

public class TableInfo
{
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public SheetInfo MainSheet { get; set; } = new();
    public List<SheetInfo> SubSheetInfos { get; } = new();
}
