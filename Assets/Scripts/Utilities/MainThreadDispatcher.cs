using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour, IMainThreadDispatcher
{
    private static MainThreadDispatcher _instance;
    public static MainThreadDispatcher Instance { get; private set; }
    private SemaphoreSlim DispatcherSemaphore { get; set; }

    private static Queue<Action> enqueuedActions = new Queue<Action>();

    private void Awake()
    {
        DispatcherSemaphore = new SemaphoreSlim(1); //1 Semaphore Slim is enough
        _instance = GameObject.FindFirstObjectByType<MainThreadDispatcher>().GetComponent<MainThreadDispatcher>();  
    }

    private void Start()
    {
        Instance = _instance;
    }

    public void Update()
    {
        Dispatcher(DispatcherSemaphore, enqueuedActions);
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
                    Debug.Log($"Action {action}");

                    enqueuedActions.Enqueue(action);
                }
            }

        }catch (Exception ex)
        {
            Debug.LogException(ex);

            throw;
        }

        return Task.CompletedTask;
    }

    public Task Dispatcher(SemaphoreSlim dispatcherSlim, Queue<Action> actionQueue)
    {
        while (dispatcherSlim.CurrentCount > 0)
        {
            dispatcherSlim.WaitAsync();

            Debug.Log(dispatcherSlim.CurrentCount);

            lock (enqueuedActions)
            {
                if(enqueuedActions.Count > 0)
                {
                    Debug.Log("HERE");

                    actionQueue.Dequeue().Invoke(); //invoke the action and dispatch it to the main thread
                }
            }

            dispatcherSlim.Release();
        }

        return Task.CompletedTask;
    }
}