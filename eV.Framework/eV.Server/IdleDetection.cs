// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using eV.EasyLog;
namespace eV.Server
{
    public class IdleDetection
    {
        private readonly int _threshold;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _task;

        public IdleDetection(int threshold)
        {
            _threshold = threshold;
            _cancellationTokenSource = new CancellationTokenSource();
            _task = new Task(Check, _cancellationTokenSource.Token);
        }
        public void Start()
        {
            _task.Start();
            Logger.Info("Idle detection start");
        }
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }
        private void Check()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                Thread.Sleep(15 * 60 * 1000);
                foreach (var (_, session) in SessionDispatch.Instance.SessionManager.GetAllActiveSession())
                {
                    var flagDateTime = session.LastActiveDateTime?.AddSeconds(_threshold);
                    if (flagDateTime < DateTime.Now)
                    {
                        session.Shutdown();
                    }
                }
            }
        }
    }
}
