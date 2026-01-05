using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using System;

public abstract class BaseState<T>: MonoBehaviour, ISubject<GenericStateBundle<T>> where T : IStateBundle
{
    protected List<IObserver<GenericStateBundle<T>>> StateListeners { get; set; } = new List<IObserver<GenericStateBundle<T>>> { };

    protected GenericStateBundle<T> StateBundle { get; set; } = new GenericStateBundle<T>();

    private async void Start()
    {
        await AddEvent();

        await AddDelegator();

        await AddSubject();

        (await GetEvent()).AddListener(PingStateListeners);

        PingStateListeners(GetInitialState());
    }

    public async void PingStateListeners(GenericStateBundle<T> stateBundle)
    {
        StateBundle = stateBundle;

        foreach (IObserver<GenericStateBundle<T>> listener in StateListeners)
        {
            await NotifyObserver(listener, StateBundle, CancellationToken.None);
        }
    }

    private async Task NotifyObserver(IObserver<GenericStateBundle<T>> observer, GenericStateBundle<T> stateBundle, CancellationToken cancellationToken)
    {
        StartCoroutine((await GetDelegator()).NotifyObserver(observer, stateBundle, new ObserverContext()
        {
            EntityType = typeof(BaseState<T>).ToString()

        }, cancellationToken));
    }

    public async void OnNotifySubject(IObserver<GenericStateBundle<T>> observer, ObserverContext context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StateListeners.Add(observer);

        await NotifyObserver(observer, StateBundle, cancellationToken);
    }

    protected abstract Task AddSubject();

    protected abstract Task AddDelegator();

    protected abstract Task AddEvent();

    protected abstract Task<UnityEvent<GenericStateBundle<T>>> GetEvent(); 

    protected abstract Task<BaseDelegator<GenericStateBundle<T>>> GetDelegator();

    protected abstract GenericStateBundle<T> GetInitialState();
}

public abstract class BaseState<T, Z> : MonoBehaviour, ISubject<GenericStateBundle<T, Z>> where T : IStateBundle
{
    protected List<IObserver<GenericStateBundle<T, Z>>> StateListeners { get; set; } = new List<IObserver<GenericStateBundle<T, Z>>> { };

    protected GenericStateBundle<T, Z> StateBundle { get; set; } = new GenericStateBundle<T, Z>();

    private async void Start()
    {
        await AddEvent();

        await AddDelegator();

        await AddSubject();

        (await GetEvent()).AddListener(PingStateListeners);

        PingStateListeners(GetInitialState());
    }

    public async void PingStateListeners(GenericStateBundle<T, Z> stateBundle)
    {
        StateBundle = stateBundle;

        foreach (IObserver<GenericStateBundle<T, Z>> listener in StateListeners)
        {
            await NotifyObserver(listener, StateBundle, CancellationToken.None);
        }
    }

    private async Task NotifyObserver(IObserver<GenericStateBundle<T, Z>> observer, GenericStateBundle<T, Z> stateBundle, CancellationToken cancellationToken)
    {
        StartCoroutine((await GetDelegator()).NotifyObserver(observer, stateBundle, new ObserverContext()
        {
            EntityType = typeof(BaseState<T>).ToString()

        }, cancellationToken));
    }

    public async void OnNotifySubject(IObserver<GenericStateBundle<T, Z>> observer, ObserverContext context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StateListeners.Add(observer);

        await NotifyObserver(observer, StateBundle, cancellationToken);
    }

    protected abstract Task AddSubject();

    protected abstract Task AddDelegator();

    protected abstract Task AddEvent();

    protected abstract Task<UnityEvent<GenericStateBundle<T, Z>>> GetEvent();

    protected abstract Task<BaseDelegator<GenericStateBundle<T, Z>>> GetDelegator();

    protected abstract GenericStateBundle<T, Z> GetInitialState();
}