// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
namespace eV.GameProfile.Interface
{
    public interface IConfigParser
    {
        public Dictionary<string, object> Parser(Dictionary<string, Type> configType, Dictionary<string, string> configJsonString);
    }
}
