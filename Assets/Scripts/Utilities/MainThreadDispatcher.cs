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

    private Action DispatchEvent {  get; set; }

    private CancellationToken CancellationToken { get; set; }

    private bool RunOnMainThread { get; set; }

    [SerializeField]
    MainThreadDispatcherEvent mainThreadDispatcherEvent;

    private void Start()
    {
        mainThreadDispatcherEvent.AddListener(DispatcherListener);
    }

    private void Update()
    {
        if (RunOnMainThread)
        {
            Dispatcher(DispatchEvent, CancellationToken);

            RunOnMainThread = false;
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

    public void DispatcherListener(Action action, CancellationToken token)
    {
        if (action == null) throw new ApplicationException($"Action can't be null {action}");

        lock (this)
        {
            DispatchEvent = action;

            CancellationToken = token;

            RunOnMainThread = true;
        }
    }
}