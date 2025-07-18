using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using System;

//pass an entire bundle instead of State<T> - T here is a bundle
public abstract class BaseState<T>: MonoBehaviour, ISubject<IObserver<T>> where T : IStateBundle
{
    protected List<IObserver<T>> StateListeners { get; set; } = new List<IObserver<T>> { };

    protected T StateBundle { get; set; }
    private void Start()
    {
        GetEvent().AddListener(PingStateListeners);
    }

    public async void PingStateListeners(T stateBundle)
    {
        StateBundle = stateBundle;

        foreach (IObserver<T> listener in StateListeners)
        {
            await NotifyObserver(listener, StateBundle, CancellationToken.None);
        }
    }


    private Task NotifyObserver(IObserver<T> observer, T stateBundle, CancellationToken cancellationToken)
    {
        StartCoroutine(GetDelegator().NotifyObserver(observer, stateBundle, new NotificationContext()
        {
            SubjectType = typeof(BaseState<T>).ToString()

        }, cancellationToken));

        return Task.CompletedTask;
    }

    public async void OnNotifySubject(IObserver<T> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        Debug.Log("Base Class On Notify through child class!");

        StateListeners.Add(observer);

        await NotifyObserver(observer, StateBundle, cancellationToken);
    }

    protected abstract void AddSubject();

    protected abstract UnityEvent<T> GetEvent(); 

    protected abstract BaseDelegatorEnhanced<T> GetDelegator();
}