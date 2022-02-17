// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

namespace eV.ServerExample.GameProfileStructure;

public class GameProfile
{
    public static GameProfile Instance { get; } = new();
    private GameProfile()
    {
    }
}
