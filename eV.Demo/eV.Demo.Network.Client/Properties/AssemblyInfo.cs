// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Reflection;
using System.Runtime.InteropServices;
using log4net.Config;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyDescription("")]
[assembly: AssemblyCopyright("Copyright ©  2020")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyCompanyAttribute("eV.Demo.Network.Client")]
[assembly: AssemblyConfigurationAttribute("Debug")]
[assembly: AssemblyFileVersionAttribute("1.0.0.0")]
[assembly: AssemblyInformationalVersionAttribute("1.0.0")]
[assembly: AssemblyProductAttribute("eV.Demo.Network.Client")]
[assembly: AssemblyTitleAttribute("eV.Demo.Network.Client")]
[assembly: AssemblyVersionAttribute("1.0.0.0")]


[assembly: ComVisible(false)]

[assembly: Guid("b2e1fd9f-3864-4b44-97ee-efa1d225ce51")]


// 指定log4net 的配置文件
[assembly: XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
