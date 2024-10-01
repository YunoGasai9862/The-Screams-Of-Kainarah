using UnityEngine;
using System.Threading.Tasks;

public class GameLoadManager: MonoBehaviour, IGameLoadManager
{
    [SerializeField]
    GameLoad gameLoad;

    [SerializeField]
    EntityPoolEvent entityPoolEvent;

    private async void Awake()
    {
        await InstantiateAndPoolGameLoad(gameLoad, entityPoolEvent);
        //use game load manager to signal to preload manager that it can proceed wtih preloading
    }

    public async Task InstantiateAndPoolGameLoad(GameLoad gameLoad, EntityPoolEvent entityPoolEvent)
    {
        GameLoad gameLoadObject = Instantiate(gameLoad);

        EntityPool<UnityEngine.GameObject> entityPool = await EntityPool<UnityEngine.GameObject>.From(gameLoadObject.name, gameLoadObject.tag, gameLoadObject.gameObject);

        await entityPoolEvent.Invoke(entityPool);
    }
}