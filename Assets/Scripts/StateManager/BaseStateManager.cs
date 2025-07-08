using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

public abstract class StateManager<T>: ISubject<IObserver<T>>
{
    protected List<IObserver<T>> StateListeners { get; set; } = new List<IObserver<T>> { };

    [SerializeField]
    public StateEvent stateEvent;

    [SerializeField]
    public GlobalGameStateDelegator globalGameStateDelegator;

    protected T State { get; set; }
    private void Start()
    {
        StateEvent.AddListener(PingStateListeners);
    }

    public async void PingStateListeners(T gameState)
    {
        GlobalGameState = gameState;

        foreach (IObserver<GameState> listener in GameStateListeners)
        {
            await NotifyObserver(listener, gameState, CancellationToken.None);
        }
    }


    private Task NotifyObserver(IObserver<GameState> observer, GameState gameState, CancellationToken cancellationToken)
    {
        StartCoroutine(globalGameStateDelegator.NotifyObserver(observer, gameState, new NotificationContext()
        {
            SubjectType = typeof(GlobalGameStateManager).ToString()

        }, cancellationToken));

        return Task.CompletedTask;
    }

    public void OnNotifySubject(IObserver<T> observer, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        GameStateListeners.Add(observer);

        await NotifyObserver(data, GlobalGameState, cancellationToken);
    }

    protected abstract void AddSubject();
}