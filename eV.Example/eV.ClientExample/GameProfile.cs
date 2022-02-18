// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.ClientExample.Object.Profile;
namespace eV.ClientExample;

public class GameProfile
{
    public static GameProfile Instance { get; } = new();
    private GameProfile()
    {
    }

    public HelloProfile? HelloProfile { get; set; }
}
