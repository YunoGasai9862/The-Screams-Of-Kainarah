using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour, IMainThreadDispatcher
{
    private static MainThreadDispatcher _instance;
    public static MainThreadDispatcher Instance 
    { 
        get 
        {
            if (_instance == null)
            {
                _instance = new GameObject("MainThreadDispatcher").AddComponent<MainThreadDispatcher>();

                DontDestroyOnLoad(Instance.gameObject);
            }

            return _instance;
        }
    }
    private CancellationTokenSource CancellationTokenSource { get; set; }
    private CancellationToken CancellationToken { get; set; }

    private SemaphoreSlim DispatcherSemaphore { get; set; }

    private static Queue<Action> enqueuedActions = new Queue<Action>();

    private void Awake()
    {
        CancellationTokenSource = new CancellationTokenSource();

        CancellationToken = CancellationTokenSource.Token;

        DispatcherSemaphore = new SemaphoreSlim(1); //1 Semaphore Slim is enough
    }

    public void Update()
    {

        Dispatcher(DispatcherSemaphore, enqueuedActions, CancellationToken);
    }

    public Task Enqueue(Action action, CancellationToken cancellationToken)
    {
        Debug.Log($"Enqueuing {action}");

        try
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                lock (enqueuedActions) //restricts enqueuedActions to be used by single thread at a time
                {
                    enqueuedActions.Enqueue(action);
                }
            }

        }catch (Exception ex)
        {
            Debug.LogException(ex);

            CancellationTokenSource.Cancel();

            throw;
        }

        return Task.CompletedTask;
    }

    public Task Dispatcher(SemaphoreSlim dispatcherSlim, Queue<Action> actionQueue, CancellationToken cancellationToken)
    {
        dispatcherSlim.WaitAsync(cancellationToken);

        while (dispatcherSlim.CurrentCount > 0)
        {
            lock (enqueuedActions)
            {
                enqueuedActions.Dequeue().Invoke(); //invoke the action and dispatch it to the main thread
            }

            dispatcherSlim.Release();
        }

        return Task.CompletedTask;
    }
}