using System;
using System.Collections.Generic;
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

    public async Task PreloadAssetWithAction<T, TAction>(AssetReference assetReference, EntityType entityType, Action<TAction> action, TAction value)
    {
        await PreloadAsset<T>(assetReference, entityType);

        action.Invoke(value);
    }

    public async Task PreloadAssetWithAction<T>(AssetReference assetReference, EntityType entityType, Action action)
    {
        await PreloadAsset<T>(assetReference, entityType);

        action.Invoke();
    }

    public async Task PreloadAsset<T>(AssetReference assetReference, EntityType entityType)
    {
        Debug.Log($"Asset Reference: {assetReference} EntityType: {entityType}");

       // await PooledGameLoad.PreloadAsset<T>(assetReference, entityType);
    }

    private void ObjectPoolActiveEventListener(ObjectPool objectPoolReference)
    {
        ObjectPoolReference = objectPoolReference;
    }

    private async void GameLoadPoolEventListener(bool value)
    {
        EntityPool = await ObjectPoolReference.GetEntityPool(Constants.GAME_PRELOAD);

        Debug.Log($"EntityPool: {EntityPool.ToString()}");

        if (EntityPool.Entity.GetComponent<GameLoad>() == null)
        {
            throw new ApplicationException("Game Load Not Found!");
        }

        PooledGameLoad = EntityPool.Entity.GetComponent<GameLoad>();
    }
}