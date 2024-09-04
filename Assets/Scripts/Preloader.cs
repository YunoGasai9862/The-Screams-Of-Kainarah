using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Preloader: MonoBehaviour, IPreloadWithAction, IPreloadWithGenericAction
{
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
        await SceneSingleton.GetGameLoader().PreloadAsset<T>(assetReference, entityType);
    }
}