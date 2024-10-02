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

    private async void Awake()
    {
        await InstantiateAndPoolGameLoad(gameLoad, entityPoolEvent);
    }

    private void Start()
    {
        gameLoadPoolEvent.AddListener(GameLoadPoolEventListener);
    }

    public async Task InstantiateAndPoolGameLoad(GameLoad gameLoad, EntityPoolEvent entityPoolEvent)
    {
        GameLoad gameLoadObject = Instantiate(gameLoad);

        EntityPool<UnityEngine.GameObject> entityPool = await EntityPool<UnityEngine.GameObject>.From(gameLoadObject.name, gameLoadObject.tag, gameLoadObject.gameObject);

        await entityPoolEvent.Invoke(entityPool);
    }

    public void GameLoadPoolEventListener()
    {
        executingPreloadingEvent.Invoke();
    }
}