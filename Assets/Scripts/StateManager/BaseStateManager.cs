using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public abstract class StateManager<T>: MonoBehaviour, ISubject<IObserver<SystemState<T>>>
{
    protected List<IObserver<SystemState<T>>> StateListeners { get; set; } = new List<IObserver<SystemState<T>>> { };

    protected SystemState<T> State { get; set; }
    private void Start()
    {
        GetEvent().AddListener(PingStateListeners);
    }

    public async void PingStateListeners(SystemState<T> gameState)
    {
        State = gameState;

        foreach (IObserver<SystemState<T>> listener in StateListeners)
        {
            await NotifyObserver(listener, gameState, CancellationToken.None);
        }
    }


    private Task NotifyObserver(IObserver<SystemState<T>> observer, SystemState<T> gameState, CancellationToken cancellationToken)
    {
        StartCoroutine(GetDelegator().NotifyObserver(observer, gameState, new NotificationContext()
        {
            SubjectType = typeof(StateManager<T>).ToString()

        }, cancellationToken));

        return Task.CompletedTask;
    }

    public async void OnNotifySubject(IObserver<SystemState<T>> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StateListeners.Add(observer);

        await NotifyObserver(observer, State, cancellationToken);
    }

    protected abstract void AddSubject();

    protected abstract UnityEvent<SystemState<T>> GetEvent();

    protected abstract BaseDelegatorEnhanced<SystemState<T>> GetDelegator();
}