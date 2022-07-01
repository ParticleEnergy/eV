// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

using eV.Module.Routing.Interface;
namespace eV.Framework.Unity;

public delegate void Handler<in T>(ISession session, T content);
