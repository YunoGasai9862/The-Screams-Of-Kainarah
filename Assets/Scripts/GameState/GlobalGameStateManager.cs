using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class GlobalGameStateManager: MonoBehaviour, ISubject<IObserver<GameState>>
{
    private List<IObserver<GameState>> GameStateListeners { get; set; } = new List<IObserver<GameState>> { };

    [SerializeField]
    public GameStateEvent gameStateEvent;

    [SerializeField]
    public GlobalGameStateDelegator globalGameStateDelegator;

    private GameState GlobalGameState { get; set; } = GameState.FREE_MOVEMENT;


    private void Start()
    {
        gameStateEvent.AddListener(PingGameStateListeners);

        globalGameStateDelegator.AddToSubjectsDict(typeof(GlobalGameStateManager).ToString(), gameObject.name, new Subject<IObserver<GameState>>());

        globalGameStateDelegator.GetSubsetSubjectsDictionary(typeof(GlobalGameStateManager).ToString())[gameObject.name].SetSubject(this);

    }

    public async void PingGameStateListeners(GameState gameState)
    {
        GlobalGameState = gameState;

        foreach (IObserver<GameState> listener in GameStateListeners)
        {
            await NotifyObserver(listener, gameState, CancellationToken.None);
        }
    }

    public async void OnNotifySubject(IObserver<GameState> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        GameStateListeners.Add(data);

        await NotifyObserver(data, GlobalGameState, cancellationToken);
    }

    private Task NotifyObserver(IObserver<GameState> observer, GameState gameState, CancellationToken cancellationToken)
    {
        StartCoroutine(globalGameStateDelegator.NotifyObserver(observer, gameState, new NotificationContext()
        {
            SubjectType = typeof(GlobalGameStateManager).ToString()

        }, cancellationToken));

        return Task.CompletedTask;
    }
}