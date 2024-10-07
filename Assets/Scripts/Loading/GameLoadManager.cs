using UnityEngine;
using System;
using System.Threading.Tasks;
using static IDeletegate;

public class GameLoadManager: MonoBehaviour, IGameLoadManager, IDeletegate
{
    [SerializeField]
    GameObject gameLoad;

    [SerializeField]
    EntityPoolEvent entityPoolEvent;

    [SerializeField]
    ExecutePreloadingEvent executePreloadingEvent;

    public InvokeMethod InvokeCustomMethod { get ; set ; }

    private async void Start()
    {
        Debug.Log("GameLoadManager Started");

        gameLoad =  await InstantiateAndPoolGameLoad(gameLoad, entityPoolEvent);

        await SetGameLoadAsChild(gameLoad);

        InvokeCustomMethod += InvokePreloading;
    }

    private void InvokePreloading()
    {
        executePreloadingEvent.Invoke();
    }

    public async Task<GameObject> InstantiateAndPoolGameLoad(GameObject gameLoad, EntityPoolEvent entityPoolEvent)
    {
        try
        {
            GameObject gameLoadObject = Instantiate(gameLoad);

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

    private Task SetGameLoadAsChild(GameObject gameLoad)
    {
        gameLoad.transform.parent = gameObject.transform;

        return Task.CompletedTask;
    }
}