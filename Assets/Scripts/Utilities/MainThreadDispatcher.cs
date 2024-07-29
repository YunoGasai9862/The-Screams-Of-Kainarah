using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

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

    private async void Update()
    {
        await DispatchIterator(DispatchActions);
    }

    public Task DispatchIterator(Queue<Action> actions)
    {
        while(actions.Count > 0)
        {
            Dispatcher(DispatchActions.Dequeue(), CancellationToken);
        }

        return Task.CompletedTask;
    }

    public Task Dispatcher(Action<object> action, object parameter = null ,CancellationToken cancellationToken)
    {
        if(DispatcherSemaphore.CurrentCount > 0 && !cancellationToken.IsCancellationRequested)
        {
            DispatcherSemaphore.WaitAsync();

            try
            {
                lock (action)
                {
                    action.Invoke(parameter); //invoke the action and dispatch it to the main thread
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