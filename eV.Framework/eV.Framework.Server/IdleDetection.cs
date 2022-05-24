// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using eV.Module.EasyLog;
using eV.Module.Session;
namespace eV.Framework.Server;

public class IdleDetection
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Task _task;
    private readonly int _threshold;

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
            foreach ((string _, Session? session) in SessionDispatch.Instance.SessionManager.GetAllActiveSession())
            {
                DateTime? flagDateTime = session.LastActiveDateTime?.AddSeconds(_threshold);
                if (flagDateTime < DateTime.Now)
                    session.Shutdown();
            }
        }
    }
}
