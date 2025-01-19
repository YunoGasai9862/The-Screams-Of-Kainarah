using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Preloader: MonoBehaviour, IPreloadWithAction, IPreloadWithGenericAction, IObserverAsync<SceneSingleton>
{

    [SerializeField]
    SceneSingletonDelegator sceneSingletonDelegator;

    private GameLoad PooledGameLoad { get; set; }
    private EntityPool EntityPool { get; set; }
    private EntityPoolManager EntityPoolManagerReference { get; set; }


    private async void Awake()
    {
        await sceneSingletonDelegator.NotifySubject(this);
    }

    public Task ExecuteAction<TAction>(Action<TAction> action, TAction value)
    {
        action.Invoke(value);

        return Task.CompletedTask;
    }

    public Task ExecuteGenericAction(Action action)
    {
        action.Invoke();

        return Task.CompletedTask;
    }

    public async Task<UnityEngine.Object> PreloadAsset<T, Z>(Z label, Asset asset) where T: UnityEngine.Object
    {
        return await PooledGameLoad.PreloadAsset<T, Z>(label, asset);
    }

    public async Task<List<UnityEngine.Object>> PreloadAssets<Z>(Z label, Asset asset)
    {
        return await PooledGameLoad.PreloadAssets<Z>(label, asset);
    }

    private async Task InitializePoolObjects(SceneSingleton sceneSingleton)
    {
        EntityPoolManagerReference = sceneSingleton.EntityPoolManager;

        EntityPool = await EntityPoolManagerReference.GetPooledEntity(Constants.GAME_PRELOAD);

        if (((GameObject)(EntityPool.Entity)).GetComponent<GameLoad>() == null)
        {
            throw new ApplicationException("Game Load Not Found!");
        }

        PooledGameLoad = ((GameObject)EntityPool.Entity).GetComponent<GameLoad>();
    }

    public async Task OnNotify(SceneSingleton data, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return;
        }

        await InitializePoolObjects(data);
    }
}