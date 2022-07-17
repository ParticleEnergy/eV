// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Tool.ExcelToJson.Excel;

public class FieldInfo
{
    public int Index { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}
