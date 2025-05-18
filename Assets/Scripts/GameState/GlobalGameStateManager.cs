using System.Collections.Generic;
using System.Threading;
using UnityEngine;

//make sure this gets live beforehand - perhaps add it in the preloading scheme
//no need to specify observer as there can be many
[ObserverSystem(SubjectType = typeof(GlobalGameState))]
public class GlobalGameState: MonoBehaviour, ISubject<IObserver<GameState>>
{
    private List<IObserver<GameState>> GameStateListeners { get; set; } = new List<IObserver<GameState>> { };

    [SerializeField]
    public GameStateEvent gameStateEvent;

    [SerializeField]
    public GlobalGameStateDelegator globalGameStateDelegator;


    private void Start()
    {
        gameStateEvent.AddListener(PingGameStateListeners);

        globalGameStateDelegator.AddToSubjectsDict(typeof(GlobalGameState).ToString(), gameObject.name, new Subject<IObserver<GameState>>());

        globalGameStateDelegator.GetSubsetSubjectsDictionary(typeof(GlobalGameState).ToString())[gameObject.name].SetSubject(this);
    }

    public void PingGameStateListeners(GameState gameState)
    {
        foreach(IObserver<GameState> listener in GameStateListeners)
        {
            //we ping all the observers
            globalGameStateDelegator.NotifyObserver(listener, gameState, new NotificationContext()
            {
                SubjectType = typeof(GlobalGameState).ToString()

            }, CancellationToken.None);
        }
    }

    public void OnNotifySubject(IObserver<GameState> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        //add the observer :)
        GameStateListeners.Add(data);
    }
}