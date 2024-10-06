using UnityEngine;
using System;
using System.Threading.Tasks;

public class GameLoadManager: MonoBehaviour, IGameLoadManager
{
    [SerializeField]
    GameObject gameLoad;

    [SerializeField]
    EntityPoolEvent entityPoolEvent;

    [SerializeField]
    ExecutePreloadingEvent executePreloadingEvent;

    public delegate void InvokePreloadingExecution();

    public InvokePreloadingExecution invokePreloadingExecution;

    private async void Start()
    {
        Debug.Log("GameLoadManager Started");

        gameLoad =  await InstantiateAndPoolGameLoad(gameLoad, entityPoolEvent);

        //make it child of manager

        invokePreloadingExecution += InvokePreloading;

    }

    public void InvokePreloading()
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
}