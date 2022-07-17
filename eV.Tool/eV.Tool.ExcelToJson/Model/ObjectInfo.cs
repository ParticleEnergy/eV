// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Tool.ExcelToJson.Define;
namespace eV.Tool.ExcelToJson.Model;

public class ObjectInfo
{
    public bool IsDependencies { get; set; }
    public bool IsMain { get; set; }
    public string Head { get; set; } = string.Empty;
    public string NamespaceName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string ProfileType { get; set; } = string.Empty;
    public string ProfileDetailType { get; set; } = string.Empty;
    public List<ObjectBaseProperty> ObjectBaseProperties { get; } = new();
    public List<ObjectComplexProperty> ObjectComplexProperties { get; } = new();

    public override string ToString()
    {
        string result;
        if (IsMain)
        {
            result = IsDependencies ? string.Format(Template.ProfileObject, Head, NamespaceName, NamespaceName, ClassName, ProfileType, ProfileDetailType, ClassName, ClassName, GetProperties()) : string.Format(Template.ProfileObjectNoDependencies, Head, NamespaceName, ClassName, ProfileType, ProfileDetailType, ClassName, ClassName, GetProperties());
        }
        else
        {
            result = string.Format(Template.ItemObject, Head, NamespaceName, ClassName, GetProperties());
        }
        return result;
    }

    private string GetProperties()
    {
        List<string> properties = new();

        properties.AddRange(ObjectBaseProperties.Select(objectBaseProperty => string.Format(Template.BaseProperty, objectBaseProperty.Comment, objectBaseProperty.Type, objectBaseProperty.Name, objectBaseProperty.DefaultValue)));

        properties.AddRange(ObjectComplexProperties.Select(objectComplexProperty => string.Format(Template.ComplexProperty, objectComplexProperty.Comment, objectComplexProperty.Type, objectComplexProperty.Name)));

        return string.Join("\n", properties);
    }
}
