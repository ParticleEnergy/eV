// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using eV.Session;
using log4net;
namespace eV.Server
{
    public class IdleDetection
    {
        private readonly ILog _logger = LogManager.GetLogger(DefaultSetting.LoggerName);
        private readonly SessionManager _sessionManager;
        private readonly int _threshold;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _task;

        public IdleDetection(SessionManager sessionManager, int threshold)
        {
            _sessionManager = sessionManager;
            _threshold = threshold;
            _cancellationTokenSource = new CancellationTokenSource();
            _task = new Task(Check, _cancellationTokenSource.Token);
        }
        public void Start()
        {
            _task.Start();
            _logger.Info("Idle detection start");
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
                foreach (var (_, session) in _sessionManager.GetAllActiveSession())
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
