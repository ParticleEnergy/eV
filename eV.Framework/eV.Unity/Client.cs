// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.


using eV.Network.Client;
using eVNetworkClient = eV.Network.Client.Client;
namespace eV.Unity
{
    public class Client
    {
        private readonly eVNetworkClient _client;
        public Client(UnitySetting setting)
        {
            ClientSetting clientSetting = new()
            {
                Address = setting.Host,
                Port = setting.Port,
                ReceiveBufferSize = setting.ReceiveBufferSize
            };
            _client = new eVNetworkClient(clientSetting);
        }
    }
}
