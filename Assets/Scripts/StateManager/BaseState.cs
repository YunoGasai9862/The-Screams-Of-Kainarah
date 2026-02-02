using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Assets.Scripts.Interfaces.Mediator;

public abstract class BaseState<T>: MonoBehaviour, ISubject<GenericStateBundle<T>> where T : IStateBundle
{
    protected List<IObserver<GenericStateBundle<T>>> StateListeners { get; set; } = new List<IObserver<GenericStateBundle<T>>> { };

    protected GenericStateBundle<T> StateBundle { get; set; } = new GenericStateBundle<T>();

    private Delegator Delegator { get; set; }

    private async void Start()
    {
        Delegator = await Helper.GetDelegator<Delegator>();

        await AddEvent();

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
        Delegator.NotifyObserversWrapper(observer, stateBundle, new ObserverContext()
        {
            SubjectType = typeof(BaseState<T>)

        }, cancellationToken);
    }

    public async void OnNotifySubject(IObserver<GenericStateBundle<T>> observer, ObserverContext context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StateListeners.Add(observer);

        await NotifyObserver(observer, StateBundle, cancellationToken);
    }

    protected abstract Task AddSubject();

    protected abstract Task AddEvent();

    protected abstract Task<UnityEvent<GenericStateBundle<T>>> GetEvent(); 

    protected abstract GenericStateBundle<T> GetInitialState();
}


public abstract class BaseState<T, Z> : MonoBehaviour, IRequest<GenericStateBundle<T, Z>> where T : IStateBundle
{
    protected List<INotify<GenericStateBundle<T, Z>>> StateListeners { get; set; } = new List<INotify<GenericStateBundle<T, Z>>> { };

    protected GenericStateBundle<T, Z> StateBundle { get; set; } = new GenericStateBundle<T, Z>();

    private Delegator Delegator { get; set; }

    private async void Start()
    {
        Delegator = await Helper.GetDelegator<Delegator>();

        await AddEvent();

        (await GetEvent()).AddListener(PingStateListeners);

        PingStateListeners(GetInitialState());
    }

    public IEnumerator PingStateListeners(GenericStateBundle<T, Z> stateBundle)
    {
        StateBundle = stateBundle;

        foreach (INotify<GenericStateBundle<T, Z>> listener in StateListeners)
        {
            //single one??
             StartCoroutine(Delegator.NotifyObserver(listener, StateBundle));
        }
    }

    private IEnumerator NotifyObservers(IRequest<GenericStateBundle<T, Z>> subject, GenericStateBundle<T, Z> stateBundle)
    {
        yield return StartCoroutine(Delegator.NotifyObservers(new SubjectContext<GenericStateBundle<T, Z>>()
        {
            EntityType = typeof(BaseState<T, Z>),
            Data = stateBundle

        }, subject));
    }

    public IEnumerator<GenericStateBundle<T, Z>> Request()
    {
        yield return StartCoroutine(Delegator.NotifyObservers(new SubjectContext<GenericStateBundle<T, Z>>()
        {
            EntityType = typeof(BaseState<T, Z>),
            Data = stateBundle

        }, subject));
    }

    protected abstract Task AddEvent();

    protected abstract Task<UnityEvent<GenericStateBundle<T, Z>>> GetEvent();

    protected abstract GenericStateBundle<T, Z> GetInitialState();
}