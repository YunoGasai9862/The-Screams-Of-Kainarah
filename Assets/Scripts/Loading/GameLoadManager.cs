using UnityEngine;
using System;
using System.Threading.Tasks;
using static IDelegate;

public class GameLoadManager: MonoBehaviour, IGameLoadManager, IDelegate
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
        gameLoad =  await InstantiateAndPoolGameLoad(gameLoad, entityPoolEvent);

        await HelperFunctions.SetAsParent(gameLoad, gameObject);

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

}