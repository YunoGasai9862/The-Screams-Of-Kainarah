using UnityEngine;
using System;
using System.Threading.Tasks;

public class GameLoadManager: MonoBehaviour, IGameLoadManager
{
    [SerializeField]
    GameObject gameLoad;

    [SerializeField]
    GameLoadPoolEvent gameLoadPoolEvent;

    [SerializeField]
    EntityPoolEvent entityPoolEvent;

    [SerializeField]
    ExecutePreloadingEvent executePreloadingEvent;

 
    private async void Start()
    {
        Debug.Log("GameLoadManager Started");

        await gameLoadPoolEvent.AddListener(GameLoadPoolEventListener);

        gameLoad = await InstantiateAndPoolGameLoad(gameLoad, entityPoolEvent);

    }

    public async Task<GameObject> InstantiateAndPoolGameLoad(GameObject gameLoad, EntityPoolEvent entityPoolEvent)
    {
        try
        {
            GameObject gameLoadObject = Instantiate(gameLoad.gameObject);

            EntityPool<UnityEngine.GameObject> entityPool = await EntityPool<UnityEngine.GameObject>.From(gameLoadObject.name, gameLoadObject.tag, gameLoadObject);

            await entityPoolEvent.Invoke(entityPool);

            return gameLoadObject;

        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

        return null;
    }

    public void GameLoadPoolEventListener()
    {
        Debug.Log("Here");

        executePreloadingEvent.Invoke();
    }
}