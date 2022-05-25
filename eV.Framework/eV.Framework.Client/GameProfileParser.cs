// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.Module.GameProfile.Interface;
using Newtonsoft.Json;
namespace eV.Framework.Client;

public class GameProfileParser : IConfigParser
{
    public GameProfileParser()
    {
        JsonSerializerSettings jsonSerializerSettings = new();
        JsonConvert.DefaultSettings = () =>
        {
            jsonSerializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
            jsonSerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            return jsonSerializerSettings;
        };
    }

    public Dictionary<string, object> Parser(Dictionary<string, Type> configType, Dictionary<string, string> configJsonString)
    {
        Dictionary<string, object> config = new();
        foreach ((string? name, Type? type) in configType)
        {
            if (!configJsonString.TryGetValue(name, out string? json))
                continue;
            object? result = JsonConvert.DeserializeObject(json, type);
            if (result != null)
                config.Add(name, result);
        }
        return config;
    }
}