// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Storage.Redis.Interface;
namespace eV.Framework.Server.Options;

public class AddressOptions : IAddress
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 8888;
}
