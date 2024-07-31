namespace FlatlinerDOA.Controls;
using System;

public static class Throttler
{
    public static Action<T> Create<T>(Action<T> action, TimeSpan throttleDelay)
    {
        if (throttleDelay == TimeSpan.Zero)
        {
            return action;
        }

        CancellationTokenSource? cts = null;
        DateTime lastExecutionTime = DateTime.MinValue;
        T? latestArg = default;
        object lockObject = new object();

        return async (T arg) =>
        {
            lock (lockObject)
            {
                latestArg = arg;
                cts?.Cancel();
                cts = new CancellationTokenSource();
            }

            var token = cts.Token;
            var delay = throttleDelay - (DateTime.UtcNow - lastExecutionTime);

            if (delay <= TimeSpan.Zero)
            {
                lastExecutionTime = DateTime.UtcNow;
                action(arg);
            }
            else
            {
                try
                {
                    await Task.Delay(delay, token);
                    lock (lockObject)
                    {
                        if (!token.IsCancellationRequested)
                        {
                            lastExecutionTime = DateTime.UtcNow;
                            // Use the latest argument
                            arg = latestArg!;
                        }
                    }
                    if (!token.IsCancellationRequested)
                    {
                        action(arg);
                    }
                }
                catch (OperationCanceledException)
                {
                    // Task was cancelled, do nothing
                }
            }
        };
    }
}
