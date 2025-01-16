using UnityEngine;
using System;
using System.Threading.Tasks;
using static IDelegate;
using System.Threading;

public class GameLoadManager: MonoBehaviour, IGameLoadManager, IDelegate, IObserverAsync<SceneSingleton>
{
    [SerializeField]
    GameObject gameLoad;

    [SerializeField]
    SceneSingletonDelegator sceneSingletonDelegator;

    [SerializeField]
    ExecutePreloadingEvent executePreloadingEvent;

    public InvokeMethod InvokeCustomMethod { get ; set ; }

    private void Start()
    {
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

    private async Task Run()
    {
        gameLoad = await InstantiateAndPoolGameLoad(gameLoad, SceneSingleton.EntityPoolManager);

        await HelperFunctions.SetAsParent(gameLoad, gameObject);
    }

    public async Task OnNotify(SceneSingleton Data, CancellationToken _token)
    {
        await Run();
    }
}