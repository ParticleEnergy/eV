// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Tool.ExcelToJson.Define;

public static class Template8
{
    public const string ProfileObjectNoDependencies = @"{0}

using System.Collections.Generic;
using eV.Module.GameProfile.Attributes;
namespace {1}
{{
    [GameProfile]
    public class {2}Profile : {3}<{4}{5}Row>
    {{

    }}

    public class {6}Row
    {{
{7}
    }}
}}
";

    public const string ProfileObject = @"{0}

using System.Collections.Generic;
using eV.Module.GameProfile.Attributes;
using {1}.Object;
namespace {2}
{{
    [GameProfile]
    public class {3}Profile : {4}<{5}{6}Row>
    {{

    }}

    public class {7}Row
    {{
{8}
    }}
}}
";

    public const string ItemObject = @"{0}

namespace {1}.Object
{{
    public class {2}
    {{
{3}
    }}
}}
";

    public const string ItemObjectGeneric = @"{0}

using System.Collections.Generic;
namespace {1}.Object
{{
    public class {2}
    {{
{3}
    }}
}}
";

    public const string BaseProperty = @"        /// <summary>
        /// {0}
        /// </summary>
        public {1} {2} {{ get; set; }} = {3};
";

    public const string ComplexProperty = @"        /// <summary>
        /// {0}
        /// </summary>
        public {1}? {2} {{ get; set; }}
";

}
