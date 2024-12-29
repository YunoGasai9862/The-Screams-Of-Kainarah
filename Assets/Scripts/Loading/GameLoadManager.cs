using UnityEngine;
using System;
using System.Threading.Tasks;
using static IDelegate;

public class GameLoadManager: MonoBehaviour, IGameLoadManager, IDelegate
{
    [SerializeField]
    GameObject gameLoad;

    [SerializeField]
    EntityPoolManagerEvent entityPoolManagerEvent;

    [SerializeField]
    ExecutePreloadingEvent executePreloadingEvent;

    private EntityPoolManager EntityPoolManager { get; set; }

    public InvokeMethod InvokeCustomMethod { get ; set ; }

    private async void Start()
    {
        await entityPoolManagerEvent.AddListener(EntityPoolManagementEvent);

        gameLoad =  await InstantiateAndPoolGameLoad(gameLoad, EntityPoolManager);

        await HelperFunctions.SetAsParent(gameLoad, gameObject);

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
    private void EntityPoolManagementEvent(EntityPoolManager entityPoolManager)
    {
        EntityPoolManager = entityPoolManager;
    }

}