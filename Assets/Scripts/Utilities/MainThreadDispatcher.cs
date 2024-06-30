using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class MainThreadDispatcher : MonoBehaviour, IMainThreadDispatcher
{
    private SemaphoreSlim DispatcherSemaphore { get; set; } = new SemaphoreSlim(1);

    private Queue<Action> DispatchActions { get; set; } = new Queue<Action>();

    private CancellationTokenSource CancellationTokenSource { get; set; }

    private CancellationToken CancellationToken { get; set; }

    [SerializeField]
    MainThreadDispatcherEvent mainThreadDispatcherEvent;

    private void Start()
    {
        mainThreadDispatcherEvent.AddListener(DispatcherListener);

        CancellationTokenSource = new CancellationTokenSource();
        
        CancellationToken = CancellationTokenSource.Token;
    }

    private void Update()
    {
        while (DispatchActions.Count > 0)
        {
            Dispatcher(DispatchActions.Dequeue(), CancellationToken);
        }
    }

    public Task Dispatcher(Action action, CancellationToken cancellationToken)
    {
        if(DispatcherSemaphore.CurrentCount > 0 && !cancellationToken.IsCancellationRequested)
        {
            DispatcherSemaphore.WaitAsync();

            try
            {
                lock (action)
                {
                    action.Invoke(); //invoke the action and dispatch it to the main thread
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

        return null;
    }

    public void DispatcherListener(Action action)
    {
        if (action == null) throw new ApplicationException($"Action can't be null {action}");

        lock (action)
        {
            DispatchActions.Enqueue(action);
        }
    }
}