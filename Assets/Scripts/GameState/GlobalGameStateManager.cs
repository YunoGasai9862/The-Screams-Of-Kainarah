using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

//make sure this gets live beforehand - perhaps add it in the preloading scheme
public class GlobalGameState: MonoBehaviour
{
    private List<GameStateAttribute> ObjectsWithGameStateAttribute { get; set; } = new List<GameStateAttribute>();

    private List<IGameStateListener> GameStateListeners { get; set; } = new List<IGameStateListener> { };

    [SerializeField]
    public GameStateEvent gameStateEvent;

    private async void OnEnable()
    {
        ObjectsWithGameStateAttribute = await Helper.GetGameObjectsWithCustomAttributes<GameStateAttribute>();

        GameStateListeners = await ExtractGameStateListenersFromGameStateAttribute(ObjectsWithGameStateAttribute);
    }

    private void Start()
    {
        gameStateEvent.AddListener(PingGameStateListeners);
    }

    public void PingGameStateListeners(GameState gameState)
    {
    }

    private Task<List<IGameStateListener>> ExtractGameStateListenersFromGameStateAttribute(List<GameStateAttribute> objectsWithGameStateAttribute)
    {
        List<IGameStateListener> gameStateListeners = new List<IGameStateListener>();

        foreach(GameStateAttribute gameStateAttribute in objectsWithGameStateAttribute)
        {
            //use find by type first, retrieve the object and find the implementation for the interface
            //thats the cleanest and best approach
        }

        return Task.FromResult(gameStateListeners);
    }

}