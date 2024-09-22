using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Preloader: MonoBehaviour, IPreloadWithAction, IPreloadWithGenericAction
{
    [SerializeField]
    EntityPoolManagerActiveEvent entityPoolManagerActiveEvent;

    [SerializeField]
    GameLoadPoolEvent GameLoadPoolEvent;

    private GameLoad PooledGameLoad { get; set; }
    private EntityPool<GameObject> EntityPool { get; set; }
    private EntityPoolManager EntityPoolManagerReference { get; set; }


    private void OnEnable()
    {
        entityPoolManagerActiveEvent.AddListener(EntityPoolManagerActiveEventListener);

        GameLoadPoolEvent.AddListener(GameLoadPoolEventListener);
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

    public async Task<UnityEngine.Object> PreloadAsset<T>(AssetReference assetReference, EntityType entityType)
    {
         return await PooledGameLoad.PreloadAsset<T>(assetReference, entityType);
    }

    private void EntityPoolManagerActiveEventListener(EntityPoolManager entityPoolManager)
    {
        EntityPoolManagerReference = entityPoolManager;
    }

    private async void GameLoadPoolEventListener(bool value)
    {
        EntityPool = await EntityPoolManagerReference.GetEntityPool<GameObject>(Constants.GAME_PRELOAD);

        if (EntityPool.Entity.GetComponent<GameLoad>() == null)
        {
            throw new ApplicationException("Game Load Not Found!");
        }

        PooledGameLoad = EntityPool.Entity.GetComponent<GameLoad>();
    }
}