using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using System;

public abstract class BaseState<T>: MonoBehaviour, ISubject<IObserver<GenericStateBundle<T>>> where T : IStateBundle
{
    protected List<IObserver<GenericStateBundle<T>>> StateListeners { get; set; } = new List<IObserver<GenericStateBundle<T>>> { };

    protected GenericStateBundle<T> StateBundle { get; set; }
    private void Start()
    {
        GetEvent().AddListener(PingStateListeners);
    }

    public async void PingStateListeners(GenericStateBundle<T> stateBundle)
    {
        StateBundle = stateBundle;

        foreach (IObserver<GenericStateBundle<T>> listener in StateListeners)
        {
            await NotifyObserver(listener, StateBundle, CancellationToken.None);
        }
    }


    private Task NotifyObserver(IObserver<GenericStateBundle<T>> observer, GenericStateBundle<T> stateBundle, CancellationToken cancellationToken)
    {
        StartCoroutine(GetDelegator().NotifyObserver(observer, stateBundle, new NotificationContext()
        {
            SubjectType = typeof(BaseState<T>).ToString()

        }, cancellationToken));

        return Task.CompletedTask;
    }

    public async void OnNotifySubject(IObserver<GenericStateBundle<T>> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        Debug.Log("Base Class On Notify through child class!");

        StateListeners.Add(observer);

        await NotifyObserver(observer, StateBundle, cancellationToken);
    }

    protected abstract void AddSubject();

    protected abstract UnityEvent<GenericStateBundle<T>> GetEvent(); 

    protected abstract BaseDelegatorEnhanced<GenericStateBundle<T>> GetDelegator();
}