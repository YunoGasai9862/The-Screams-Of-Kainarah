using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MainThreadDispatcher : IMainThreadDispatcher
{
    private SemaphoreSlim DispatcherSemaphore { get; set; } = new SemaphoreSlim(1);

    public Task Dispatcher(Action actionQueue)
    {
        if(DispatcherSemaphore.CurrentCount > 0)
        {
            Debug.Log($"Before wait: {DispatcherSemaphore.CurrentCount}");

            DispatcherSemaphore.WaitAsync();

            Debug.Log($"After wait: {DispatcherSemaphore.CurrentCount}");

            try
            {
                lock (actionQueue)
                {
                    actionQueue?.Invoke(); //invoke the action and dispatch it to the main thread
                }

            }catch(Exception ex)
            {
                Debug.LogException(ex);

                throw;
            }
            finally
            {
                DispatcherSemaphore.Release();
            }

        }

        return Task.CompletedTask;
    }
}