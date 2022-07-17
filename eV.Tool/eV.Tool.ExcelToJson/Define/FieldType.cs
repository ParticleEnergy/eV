// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Tool.ExcelToJson.Define;

public static class FieldType
{
    public const string PrimaryKey = "Pk";
    public const string PrimaryKeyList = "PkList";
    public const string PrimaryKeyDictionary = "PkDict";

    public const string ForeignKeyObject = "FkObject";
    public const string ForeignKeyList = "FkList";
    public const string ForeignKeyDictionary = "FkDict";

    public const string String = "String";
    public const string Int = "Int";
    public const string Double = "Double";
    public const string Bool = "Bool";

    public const string ListString = "ListString";
    public const string ListInt = "ListInt";
    public const string ListDouble = "ListDouble";
    public const string ListBool = "ListBool";

    public const string Dict = "Dict";

    public const string Class = "Class";

    public static List<string> BaseTypes => new()
    {
        String,
        Int,
        Double,
        Bool
    };

    public static List<string> PrimaryKeyTypes => new()
    {
        PrimaryKey,
        PrimaryKeyList,
        PrimaryKeyDictionary
    };

    public static List<string> ForeignKeyTypes => new()
    {
        ForeignKeyObject,
        ForeignKeyList,
        ForeignKeyDictionary
    };

    public static List<string> ListTypes => new()
    {
        ListString,
        ListBool,
        ListDouble,
        ListInt
    };
}
