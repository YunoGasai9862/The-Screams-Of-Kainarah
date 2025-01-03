using UnityEngine;
using System;
using System.Threading.Tasks;
using static IDelegate;

public class GameLoadManager: MonoBehaviour, IGameLoadManager, IDelegate
{
    [SerializeField]
    GameObject gameLoad;

    [SerializeField]
    ExecutePreloadingEvent executePreloadingEvent;

    [SerializeField]
    SceneSingletonActiveEvent sceneSingletonActiveEvent;

    public InvokeMethod InvokeCustomMethod { get ; set ; }

    private async void Start()
    {
        await sceneSingletonActiveEvent.AddListener(Run);

        InvokeCustomMethod += InvokePreloading;
    }

    private void InvokePreloading()
    {
        executePreloadingEvent.Invoke();
    }

    public async Task<GameObject> InstantiateAndPoolGameLoad(GameObject gameLoad, EntityPoolManager entityPoolManager)
    {
        try
        {
            GameObject gameLoadObject = Instantiate(gameLoad);

            await entityPoolManager.Pool(await EntityPool.From(gameLoadObject.name, gameLoadObject.tag, gameLoadObject));

            return gameLoadObject;

        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

        return null;
    }
    private async void Run()
    {
        gameLoad = await InstantiateAndPoolGameLoad(gameLoad, SceneSingleton.EntityPoolManager);

        await HelperFunctions.SetAsParent(gameLoad, gameObject);
    }

}