using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Preloader: MonoBehaviour, IPreloadWithAction, IPreloadWithGenericAction
{
    public async Task PreloadAssetWithAction<T, TAction>(AssetReference assetReference, Action<TAction> action, TAction value)
    {
        await PreloadAsset<T>(assetReference);

        action.Invoke(value);
    }

    public async Task PreloadAssetWithAction<T>(AssetReference assetReference, Action action)
    {
        await PreloadAsset<T>(assetReference);

        action.Invoke();
    }

    //preload without any action
    public async Task PreloadAsset<T>(AssetReference assetReference)
    {
        await SceneSingleton.GetGameLoader().PreloadAsset<T>(assetReference);
    }
}