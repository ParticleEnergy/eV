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

    private readonly string _profileObjectNoDependencies;
    private readonly string _profileObject;

    private readonly string _baseProperty;
    private readonly string _complexProperty;
    private readonly bool _csharpVersion;

    public ObjectInfo(string csharpVersion)
    {
        _csharpVersion = csharpVersion.ToLower().Equals("lastest");
        if (csharpVersion.ToLower().Equals("lastest"))
        {
            _profileObjectNoDependencies = Template.ProfileObjectNoDependencies;
            _profileObject = Template.ProfileObject;
            _baseProperty = Template.BaseProperty;
            _complexProperty = Template.ComplexProperty;
        }
        else
        {
            _profileObjectNoDependencies = Template8.ProfileObjectNoDependencies;
            _profileObject = Template8.ProfileObject;
            _baseProperty = Template8.BaseProperty;
            _complexProperty = Template8.ComplexProperty;
        }
    }

    public override string ToString()
    {
        string result;
        if (IsMain)
        {
            result = IsDependencies
                ? string.Format(_profileObject, Head, NamespaceName, NamespaceName, ClassName, ProfileType,
                    ProfileDetailType, ClassName, ClassName, GetProperties())
                : string.Format(_profileObjectNoDependencies, Head, NamespaceName, ClassName, ProfileType,
                    ProfileDetailType, ClassName, ClassName, GetProperties());
        }
        else
        {
            result = string.Format(
                _csharpVersion ? Template.ItemObject :
                ObjectComplexProperties.Count > 0 ? Template8.ItemObjectGeneric : Template8.ItemObject, Head,
                NamespaceName, ClassName, GetProperties());
        }

        return result;
    }

    private string GetProperties()
    {
        List<string> properties = new();

        properties.AddRange(ObjectBaseProperties.Select(objectBaseProperty => string.Format(_baseProperty,
            objectBaseProperty.Comment, objectBaseProperty.Type, objectBaseProperty.Name,
            objectBaseProperty.DefaultValue)));

        properties.AddRange(ObjectComplexProperties.Select(objectComplexProperty => string.Format(_complexProperty,
            objectComplexProperty.Comment, objectComplexProperty.Type, objectComplexProperty.Name)));

        return string.Join("\n", properties);
    }
}
