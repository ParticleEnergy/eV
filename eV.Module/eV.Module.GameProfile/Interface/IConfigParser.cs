// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Module.GameProfile.Interface;

public interface IConfigParser
{
    public Dictionary<string, object> Parser(Dictionary<string, Type> configType, Dictionary<string, string> configJsonString);
}
