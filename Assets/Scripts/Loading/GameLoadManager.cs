using UnityEngine;
using System.Threading.Tasks;

public class GameLoadManager: MonoBehaviour, IGameLoadManager
{
    [SerializeField]
    GameLoad gameLoad;

    [SerializeField]
    GameLoadPoolEvent gameLoadPoolEvent;

    [SerializeField]
    EntityPoolEvent entityPoolEvent;

    [SerializeField]
    ExecutingPreloadingEvent executingPreloadingEvent;

 
    private async void Start()
    {
        Debug.Log("GameLoadManager Started");

        await gameLoadPoolEvent.AddListener(GameLoadPoolEventListener);

        gameLoad = await InstantiateAndPoolGameLoad(gameLoad, entityPoolEvent);
    }

    public async Task<GameLoad> InstantiateAndPoolGameLoad(GameLoad gameLoad, EntityPoolEvent entityPoolEvent)
    {
        GameObject gameLoadObject = Instantiate(gameLoad.gameObject);

        EntityPool<UnityEngine.GameObject> entityPool = await EntityPool<UnityEngine.GameObject>.From(gameLoadObject.name, gameLoadObject.tag, gameLoadObject);

        Debug.Log("GameLoad Instantiated");

        await entityPoolEvent.Invoke(entityPool);

        return gameLoadObject.GetComponent<GameLoad>();
    }

    public void GameLoadPoolEventListener()
    {
        Debug.Log("Here");

        executingPreloadingEvent.Invoke();
    }
}