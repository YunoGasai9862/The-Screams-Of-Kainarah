using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Preloader: MonoBehaviour, IPreloadWithAction, IPreloadWithGenericAction, IObserver<EntityPoolManager>
{
    private EntityPoolManagerDelegator m_entityPoolManagerDelegator;
    private GameLoad PooledGameLoad { get; set; }
    private EntityPool EntityPool { get; set; }

    private void Start()
    {
        m_entityPoolManagerDelegator = Helper.GetDelegator<EntityPoolManagerDelegator>();

        StartCoroutine(m_entityPoolManagerDelegator.NotifySubject(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(EntityPoolManager).ToString()), CancellationToken.None));
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

    public async Task<Object> PreloadAsset<T>(PreloadPackage preloadPackage) where T: UnityEngine.Object
    {
        return await PooledGameLoad.PreloadAsset<T>(preloadPackage);
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

    public async void OnNotify(EntityPoolManager data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        await InitializePoolObjects(data);
    }
}