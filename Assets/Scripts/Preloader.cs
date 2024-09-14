using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Preloader: MonoBehaviour, IPreloadWithAction, IPreloadWithGenericAction
{
    [SerializeField]
    ObjectPoolActiveEvent objectPoolActiveEvent;

    [SerializeField]
    GameLoadPoolEvent GameLoadPoolEvent;

    private GameLoad PooledGameLoad { get; set; }
    private EntityPool EntityPool { get; set; }
    private ObjectPool ObjectPoolReference { get; set; }


    private void OnEnable()
    {
        objectPoolActiveEvent.AddListener(ObjectPoolActiveEventListener);

        GameLoadPoolEvent.AddListener(GameLoadPoolEventListener);
    }

    public Task ExecuteAction<TAction>(Action<TAction> action, TAction value)
    {
        action.Invoke(value);

        return Task.CompletedTask;
    }

    public Task ExecuteGenericAction<T>(Action action)
    {
        action.Invoke();

        return Task.CompletedTask;
    }

    public async Task<UnityEngine.Object> PreloadAsset<T>(AssetReference assetReference, EntityType entityType)
    {
         Debug.Log($"Asset Reference: {assetReference} EntityType: {entityType}");

         return await PooledGameLoad.PreloadAsset<T>(assetReference, entityType);
    }

    private void ObjectPoolActiveEventListener(ObjectPool objectPoolReference)
    {
        ObjectPoolReference = objectPoolReference;
    }

    private async void GameLoadPoolEventListener(bool value)
    {
        EntityPool = await ObjectPoolReference.GetEntityPool(Constants.GAME_PRELOAD);

        if (EntityPool.Entity.GetComponent<GameLoad>() == null)
        {
            throw new ApplicationException("Game Load Not Found!");
        }

        PooledGameLoad = EntityPool.Entity.GetComponent<GameLoad>();
    }
}