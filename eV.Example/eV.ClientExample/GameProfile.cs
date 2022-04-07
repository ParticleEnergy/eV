// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.DataStruct.ProfileStruct;
namespace eV.ClientExample;

public class GameProfile
{
    public static GameProfile Instance { get; } = new();
    private GameProfile()
    {
    }

    public HelloProfile? HelloProfile { get; set; }
}
