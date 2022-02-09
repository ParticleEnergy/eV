// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using eV.GameProfile.Interface;
using UnityEngine;
namespace eV.Unity
{
    public class GameProfileParser : IConfigParser
    {
        public Dictionary<string, object> Parser(Dictionary<string, Type> configType, Dictionary<string, string> configJsonString)
        {
            Dictionary<string, object> config = new();
            foreach (var ct in configType)
            {
                string name = ct.Key;
                Type type = ct.Value;
                if (!configJsonString.TryGetValue(name, out string? json))
                    continue;
                object? result = JsonUtility.FromJson(json, type);
                if (result != null)
                {
                    config.Add(name, result);
                }
            }
            return config;
        }
    }
}
