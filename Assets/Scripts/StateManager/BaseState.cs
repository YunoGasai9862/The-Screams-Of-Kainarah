using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using System;

public abstract class BaseState<T>: MonoBehaviour, ISubject<IObserver<GenericState<T>>> where T : Enum
{
    protected List<IObserver<GenericState<T>>> StateListeners { get; set; } = new List<IObserver<GenericState<T>>> { };

    protected GenericState<T> State { get; set; }
    private void Start()
    {

        GetEvent().AddListener(PingStateListeners);
    }

    public async void PingStateListeners(GenericState<T> gameState)
    {
        State = gameState;

        foreach (IObserver<GenericState<T>> listener in StateListeners)
        {
            await NotifyObserver(listener, gameState, CancellationToken.None);
        }
    }


    private Task NotifyObserver(IObserver<GenericState<T>> observer, GenericState<T> gameState, CancellationToken cancellationToken)
    {
        StartCoroutine(GetDelegator().NotifyObserver(observer, gameState, new NotificationContext()
        {
            SubjectType = typeof(BaseState<T>).ToString()

        }, cancellationToken));

        return Task.CompletedTask;
    }

    public async void OnNotifySubject(IObserver<GenericState<T>> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        Debug.Log("Base Class On Notify through child class!");

        StateListeners.Add(observer);

        await NotifyObserver(observer, State, cancellationToken);
    }

    protected abstract void AddSubject();

    protected abstract UnityEvent<GenericState<T>> GetEvent(); 

    protected abstract BaseDelegatorEnhanced<GenericState<T>> GetDelegator();
}