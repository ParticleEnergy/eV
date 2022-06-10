// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Tool.ExcelToJson.Define;

public static class FieldType
{
    public const string PrimaryKey = "Pk";
    public const string ForeignKey = "Fk";
    public const string ForeignKeyList = "FkList";
    public const string ForeignKeyDictionary = "FkDictionary";

    public const string String = "String";
    public const string Int = "Int";
    public const string Double = "Double";
    public const string Bool = "Bool";

    public const string ListString = "ListString";
    public const string ListInt = "ListInt";
    public const string ListDouble = "ListDouble";
    public const string ListBool = "ListBool";
}
