// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.GameProfile.Interface;
using UnityEngine;
namespace eV.Framework.Unity;

public class GameProfileParser : IConfigParser
{
    public Dictionary<string, object> Parser(Dictionary<string, Type> configType, Dictionary<string, string> configJsonString)
    {
        Dictionary<string, object> config = new();
        foreach (KeyValuePair<string, Type> ct in configType)
        {
            string name = ct.Key;
            Type type = ct.Value;
            if (!configJsonString.TryGetValue(name, out string? json))
                continue;
            object? result = JsonUtility.FromJson(json, type);
            if (result != null)
                config.Add(name, result);
        }
        return config;
    }
}
