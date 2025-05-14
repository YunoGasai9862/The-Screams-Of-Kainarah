using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[ObserverSystem(SubjectType = typeof(GlobalGameState), ObserverType = typeof(IGameStateListener))]
public class GlobalGameState: MonoBehaviour
{
    private List<GameStateAttribute> Listeners { get; set; } = new List<GameStateAttribute>();

    [SerializeField]
    public GameStateEvent gameStateEvent;

    private async void OnEnable()
    {
        Listeners = await Helper.GetGameObjectsWithCustomAttributes<GameStateAttribute>();
    }

    private void Start()
    {
        gameStateEvent.AddListener(PingGameStateListeners);
    }

    public void PingGameStateListeners(GameState gameState)
    {

    }

}