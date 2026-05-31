using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseState<T>: MonoBehaviour, Assets.Scripts.Interfaces.Mediator.EnhancedV3.IRequest<GenericStateBundle<T>> where T : IStateBundle
{
    protected List<INotify<GenericStateBundle<T>>> StateListeners { get; set; } = new List<INotify<GenericStateBundle<T>>> { };

    protected GenericStateBundle<T> StateBundle { get; set; } = new GenericStateBundle<T>();

    protected StateEvent StateEvent { get; set; }

    private Delegator Delegator { get; set; }
    private async void Start()
    {
        StartCoroutine(Helper.GetDelegator<Delegator>(value => Delegator = value));

        StateEvent = await Helper.GetCustomEvent<StateEvent>();

        StateEvent.AddListener<GenericStateBundle<T>>(PingStateListeners);

        PingStateListeners(GetInitialState());
    }

    public void PingStateListeners(GenericStateBundle<T> bundle)
    {
        Debug.Log($"Bundle Type<T>: {bundle.GetType()} - {typeof(GenericStateBundle<T>)}");

        if (bundle.GetType() != typeof(GenericStateBundle<T>))
        {
            Debug.Log($"Bundle should be of type GenericStateBundle<T>! Incoming type: {bundle.GetType()} - Skipping!");
            return;
        }

        foreach (INotify<GenericStateBundle<T>> listener in StateListeners)
        {
            Delegator.NotifyObserverWrapper(new SubjectContext<GenericStateBundle<T>>()
            {

                EntityType = typeof(BaseState<T>),
                Data = StateBundle

            }, this, listener);
        }
    }

    protected abstract GenericStateBundle<T> GetInitialState();

    public Task<GenericStateBundle<T>> Request(INotify<GenericStateBundle<T>> obsever)
    {
        StateListeners.Add(obsever);

        StartCoroutine(Delegator.NotifyObservers(new SubjectContext<GenericStateBundle<T>>()
        {
            EntityType = typeof(BaseState<T>),
            Data = StateBundle

        }, this));

        return Task.FromResult(StateBundle);
    }
}


public abstract class BaseState<T, Z> : MonoBehaviour, Assets.Scripts.Interfaces.Mediator.EnhancedV3.IRequest<GenericStateBundle<T, Z>> where T : IStateBundle
{
    protected List<INotify<GenericStateBundle<T, Z>>> StateListeners { get; set; } = new List<INotify<GenericStateBundle<T, Z>>> { };

    protected GenericStateBundle<T, Z> StateBundle { get; set; } = new GenericStateBundle<T, Z>();

    protected StateEvent StateEvent { get; set; }

    private Delegator Delegator { get; set; }

    private async void Start()
    {
        StartCoroutine(Helper.GetDelegator<Delegator>(value => Delegator = value));

        StateEvent = await Helper.GetCustomEvent<StateEvent>();

        StateEvent.AddListener<GenericStateBundle<T, Z>>(PingStateListeners);

        PingStateListeners(GetInitialState());
    }

    public void PingStateListeners(GenericStateBundle<T, Z> bundle)
    {
        if (bundle.GetType() != typeof(GenericStateBundle<T, Z>))
        {
            Debug.Log($"Bundle should be of type GenericStateBundle<T, Z>! Incoming type: {bundle.GetType()} - Skipping!");
            return;
        }

        StateBundle = (GenericStateBundle<T, Z>)bundle;

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

    protected abstract GenericStateBundle<T, Z> GetInitialState();

    public Task<GenericStateBundle<T, Z>> Request(INotify<GenericStateBundle<T, Z>> obsever)
    {
        StateListeners.Add(obsever);

        Delegator.NotifyObserversWrapper(new SubjectContext<GenericStateBundle<T, Z>>()
        {
            EntityType = typeof(BaseState<T, Z>),
            Data = StateBundle

        }, this);

        return Task.FromResult(StateBundle);
    }
}