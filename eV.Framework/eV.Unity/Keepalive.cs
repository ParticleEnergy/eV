// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Threading;
using eV.Routing;
namespace eV.Unity
{
    public class Keepalive
    {
        private readonly int _keepAliveInterval;
        private Timer? _timer;

        public Keepalive(int keepAliveInterval)
        {
            _keepAliveInterval = keepAliveInterval;
        }

        public void Start()
        {
            _timer = new Timer(delegate
            {
                Packet packet = new();
                packet.SetName("ClientKeepalive");
                packet.SetContent(Serializer.Serialize(new
                {
                }));
                ClientSession.Send(Package.Pack(packet));
            }, 0, 0, _keepAliveInterval * 1000);
        }

        public void Stop()
        {
            _timer?.Change(-1, -1);
            _timer?.Dispose();
        }
    }
}
