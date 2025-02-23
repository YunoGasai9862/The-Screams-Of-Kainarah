using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static ExceptionList;

public class Preloader: MonoBehaviour, IPreloadWithAction, IPreloadWithGenericAction, IObserver<EntityPoolManager>
{
    private EntityPoolManagerDelegator m_entityPoolManagerDelegator;
    private GameLoad PooledGameLoad { get; set; }
    private EntityPool EntityPool { get; set; }

    private void Start()
    {
        m_entityPoolManagerDelegator = Helper.GetDelegator<EntityPoolManagerDelegator>();

        StartCoroutine(m_entityPoolManagerDelegator.NotifySubject(this));
    }

    public Task ExecuteAction<TAction>(System.Action<TAction> action, TAction value)
    {
        action.Invoke(value);

        return Task.CompletedTask;
    }

    public Task ExecuteGenericAction(System.Action action)
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

    private Task InitializePoolObjects(EntityPoolManager entityPoolManager)
    {
        EntityPool = entityPoolManager.GetPooledEntity(Constants.GAME_PRELOAD);

        if (((GameObject)(EntityPool.Entity)).GetComponent<GameLoad>() == null)
        {
            throw new System.ApplicationException("Game Load Not Found!");
        }

        PooledGameLoad = ((GameObject)EntityPool.Entity).GetComponent<GameLoad>();

        return Task.CompletedTask;
    }

    public async void OnNotify(EntityPoolManager data, NotificationContext notificationContext, params object[] optional)
    {
        await InitializePoolObjects(data); ;
    }
}