using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

//make sure this gets live beforehand - perhaps add it in the preloading scheme
//no need to specify observer as there can be many
[ObserverSystem(SubjectType = typeof(GlobalGameState))]
public class GlobalGameState: BaseDelegatorEnhanced<GlobalGameState>
{
    private List<IGameStateListener> GameStateListeners { get; set; } = new List<IGameStateListener> { };

    [SerializeField]
    public GameStateEvent gameStateEvent;

    private async void OnEnable()
    {

    }

    private void Start()
    {
        gameStateEvent.AddListener(PingGameStateListeners);
    }

    public void PingGameStateListeners(GameState gameState)
    {

    }
}