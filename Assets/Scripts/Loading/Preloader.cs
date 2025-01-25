using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Preloader: MonoBehaviour, IPreloadWithAction, IPreloadWithGenericAction, IObserver<EntityPoolManager>
{
    //use same instance!
    private EntityPoolManagerDelegator m_entityPoolManagerDelegator;
    private GameLoad PooledGameLoad { get; set; }
    private EntityPool EntityPool { get; set; }

    private void Start()
    {
        m_entityPoolManagerDelegator = GetComponentInParent<PreloaderManager>().entityPoolManagerDelegator;

        //resolve this issue, its happening with AudioPreload as well! - it's instead pointing to stale reference/old reference, hence its always null!
        Debug.Log(m_entityPoolManagerDelegator);
        Debug.Log(m_entityPoolManagerDelegator.Subject);
        Debug.Log(m_entityPoolManagerDelegator.Subject.GetSubject());

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

    public async void OnNotify(EntityPoolManager data, params object[] optional)
    {
        Debug.Log("Inside On Notify for Preloader");

        await InitializePoolObjects(data); ;
    }
}