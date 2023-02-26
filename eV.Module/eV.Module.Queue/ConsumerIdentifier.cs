// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


namespace eV.Module.Queue;

public class ConsumerIdentifier
{
    public ConsumerIdentifier(string stream, string group, string consumer)
    {
        Stream = stream;
        Group = group;
        Consumer = consumer;
    }

    public string Stream { get; }
    public string Group { get; }
    public string Consumer { get; }
}
