// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Tool.ExcelToJson.Define;

public static class Const
{
    public const string InExcelPath = "InExcelPath";
    public const string OutObjectFilePath = "OutObjectFilePath";
    public const string OutJsonFilePath = "OutJsonFilePath";
    public const string OutObjectNamespace = "OutObjectNamespace";
    public const string OutObjectFileHead = "OutObjectFileHead";

    public const string MainSheet = "Main";
    public const string SplitFlag = ",";

    public const int CommentRowIndex = 1;
    public const int NameRowIndex = 2;
    public const int TypeRowIndex = 3;
    public const int DataStartRowIndex = 4;

    public const int MainPrimaryKeyCellIndex = 0;

    public const int SubForeignKeyCellIndex = 0;
    public const int SubPrimaryKeyCellIndex = 1;
}
