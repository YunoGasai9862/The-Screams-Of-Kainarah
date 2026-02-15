using Assets.Scripts.Interfaces.Mediator.EnhancedV3;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseState<T>: MonoBehaviour, IRequest<GenericStateBundle<T>> where T : IStateBundle
{
    protected List<INotify<GenericStateBundle<T>>> StateListeners { get; set; } = new List<INotify<GenericStateBundle<T>>> { };

    protected GenericStateBundle<T> StateBundle { get; set; } = new GenericStateBundle<T>();

    private Delegator Delegator { get; set; }
    private async void Start()
    {
        Delegator = await Helper.GetDelegator<Delegator>();

        await AddEvent();

        GetEvent().AddListener(PingStateListeners);

        PingStateListeners(GetInitialState());
    }

    public void PingStateListeners(GenericStateBundle<T> stateBundle)
    {
        StateBundle = stateBundle;

        foreach (INotify<GenericStateBundle<T>> listener in StateListeners)
        {
            Delegator.NotifyObserverWrapper(new SubjectContext<GenericStateBundle<T>>()
            {

                EntityType = typeof(BaseState<T>),
                Data = StateBundle

            }, this, listener);
        }
    }

    public IEnumerator<GenericStateBundle<T>> Request(INotify<GenericStateBundle<T>> obsever)
    {
        StateListeners.Add(obsever);

        StartCoroutine(Delegator.NotifyObservers(new SubjectContext<GenericStateBundle<T>>()
        {
            EntityType = typeof(BaseState<T>),
            Data = StateBundle

        }, this));

        yield return null;
    }

    protected abstract Task AddEvent();

    protected abstract UnityEvent<GenericStateBundle<T>> GetEvent();

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

        GetEvent().AddListener(PingStateListeners);

        PingStateListeners(GetInitialState());
    }

    public void PingStateListeners(GenericStateBundle<T, Z> stateBundle)
    {
        StateBundle = stateBundle;

        foreach (INotify<GenericStateBundle<T, Z>> listener in StateListeners)
        {
            //single one (fix this later in the base)
             Delegator.NotifyObserverWrapper(new SubjectContext<GenericStateBundle<T, Z>>()
             {

                 EntityType = typeof(BaseState<T, Z>),
                 Data = StateBundle

             }, this, listener);
        }
    }

    public IEnumerator<GenericStateBundle<T, Z>> Request(INotify<GenericStateBundle<T, Z>> obsever)
    {
        StateListeners.Add(obsever);

        StartCoroutine(Delegator.NotifyObservers(new SubjectContext<GenericStateBundle<T, Z>>()
        {
            EntityType = typeof(BaseState<T, Z>),
            Data = StateBundle

        }, this));

        yield return null;
    }

    protected abstract Task AddEvent();

    protected abstract UnityEvent<GenericStateBundle<T, Z>> GetEvent();

    protected abstract GenericStateBundle<T, Z> GetInitialState();
}