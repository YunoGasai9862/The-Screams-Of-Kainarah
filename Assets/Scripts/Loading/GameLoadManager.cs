using UnityEngine;
using System;
using System.Threading.Tasks;
using static IDelegate;
using System.Threading;

public class GameLoadManager: MonoBehaviour, IGameLoadManager, IObserverAsync<EntityPoolManager>
{
    [SerializeField]
    GameObject gameLoad;

    [SerializeField]
    EntityPoolManagerDelegator entityPoolManagerDelegator;

    [SerializeField]
    ExecutePreloadingEvent executePreloadingEvent;

    private async void Start()
    {
       await entityPoolManagerDelegator.NotifySubject(this);
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

    private async Task Run(EntityPoolManager entityPoolManager)
    {
        gameLoad = await InstantiateAndPoolGameLoad(gameLoad, entityPoolManager);

        await HelperFunctions.SetAsParent(gameLoad, gameObject);
    }

    public async Task OnNotify(EntityPoolManager data, CancellationToken _token)
    {
        if (_token.IsCancellationRequested)
        {
            return;
        }

        await Run(data);

        await InvokePreloading();
    }
}