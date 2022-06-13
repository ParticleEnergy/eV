// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Tool.ExcelToJson.Define;

public static class FieldType
{
    public const string PrimaryKeyObject = "PkObject";
    public const string PrimaryKeyList = "PkList";
    public const string PrimaryKeyDictionary = "PkDict";

    public const string ForeignKey = "Fk";

    public const string ListString = "ListString";
    public const string ListInt = "ListInt";
    public const string ListDouble = "ListDouble";
    public const string ListBool = "ListBool";

    public const string String = "String";
    public const string Int = "Int";
    public const string Double = "Double";
    public const string Bool = "Bool";

    public static List<string> PrimaryKeyTypes => new()
    {
        PrimaryKeyObject,
        PrimaryKeyList,
        PrimaryKeyDictionary
    };

    public static List<string> ListTypes => new()
    {
        ListString,
        ListBool,
        ListDouble,
        ListInt
    };

    public static List<string> AllTypes => new()
    {
        PrimaryKeyObject,
        PrimaryKeyList,
        PrimaryKeyDictionary,
        ForeignKey,
        ListString,
        ListBool,
        ListDouble,
        ListInt,
        String,
        Int,
        Double,
        Bool
    };
}
