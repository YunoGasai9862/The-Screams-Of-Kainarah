using UnityEngine;
using System;
using System.Threading.Tasks;
using static IDelegate;
using System.Threading;

public class GameLoadManager: MonoBehaviour, IGameLoadManager, IObserverAsync<SceneSingleton>
{
    [SerializeField]
    GameObject gameLoad;

    [SerializeField]
    SceneSingletonDelegator sceneSingletonDelegator;

    [SerializeField]
    ExecutePreloadingEvent executePreloadingEvent;

    private async void Start()
    {
       await sceneSingletonDelegator.NotifySubject(this);
    }

    private async Task InvokePreloading()
    {
        await executePreloadingEvent.Invoke();
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

    private async Task Run(SceneSingleton sceneSingleton)
    {
        Debug.Log(sceneSingleton.EntityPoolManager);

        gameLoad = await InstantiateAndPoolGameLoad(gameLoad, sceneSingleton.EntityPoolManager);

        await HelperFunctions.SetAsParent(gameLoad, gameObject);
    }

    public async Task OnNotify(SceneSingleton data, CancellationToken _token)
    {
        if (_token.IsCancellationRequested)
        {
            return;
        }

        Debug.Log(data);

        await Run(data);

        await InvokePreloading();
    }
}