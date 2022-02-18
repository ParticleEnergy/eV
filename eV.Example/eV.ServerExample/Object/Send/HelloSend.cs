// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.Routing.Attributes;
namespace eV.ServerExample.Object.Send;

[SendMessage]
public class HelloSend
{
    public string? Text { get; set; }
}
