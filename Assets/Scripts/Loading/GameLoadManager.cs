using UnityEngine;
using System;
using System.Threading.Tasks;
using static IDelegate;
using System.Threading;

public class GameLoadManager: MonoBehaviour, IGameLoadManager, IObserver<EntityPoolManager>
{
    [SerializeField]
    GameObject gameLoad;

    [SerializeField]
    EntityPoolManagerDelegator entityPoolManagerDelegator;

    [SerializeField]
    ExecutePreloadingEvent executePreloadingEvent;

    private void Start()
    {
       StartCoroutine(entityPoolManagerDelegator.NotifySubject(this));
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

            entityPoolManager.Pool(await EntityPool.From(gameLoadObject.name, gameLoadObject.tag, gameLoadObject));

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

    public async void OnNotify(EntityPoolManager data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        await Run(data);

        await InvokePreloading();
    }
}