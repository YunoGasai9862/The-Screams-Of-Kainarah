using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using System;

//pass an entire bundle instead of State<T>
public abstract class BaseState<T>: MonoBehaviour, ISubject<IObserver<State<T>>>
{
    protected List<IObserver<State<T>>> StateListeners { get; set; } = new List<IObserver<State<T>>> { };

    protected State<T> State { get; set; }
    private void Start()
    {
        GetEvent().AddListener(PingStateListeners);
    }

    public async void PingStateListeners(State<T> gameState)
    {
        State = gameState;

        foreach (IObserver<State<T>> listener in StateListeners)
        {
            await NotifyObserver(listener, gameState, CancellationToken.None);
        }
    }


    private Task NotifyObserver(IObserver<State<T>> observer, State<T> gameState, CancellationToken cancellationToken)
    {
        StartCoroutine(GetDelegator().NotifyObserver(observer, gameState, new NotificationContext()
        {
            SubjectType = typeof(BaseState<T>).ToString()

        }, cancellationToken));

        return Task.CompletedTask;
    }

    public async void OnNotifySubject(IObserver<State<T>> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        Debug.Log("Base Class On Notify through child class!");

        StateListeners.Add(observer);

        await NotifyObserver(observer, State, cancellationToken);
    }

    protected abstract void AddSubject();

    protected abstract UnityEvent<State<T>> GetEvent(); 

    protected abstract BaseDelegatorEnhanced<State<T>> GetDelegator();
}