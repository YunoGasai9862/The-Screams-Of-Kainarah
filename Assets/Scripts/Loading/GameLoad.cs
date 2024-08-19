using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System;

public class GameLoad : MonoBehaviour, IGameLoad
{
    //now device how you'll invoke this. The other assets might not be available.
    //maybe create a scriptable object to keep the lists of assets + action???
    //The ones that need preloading - can't preload themselves!! so they can't invoke preloadAsset method
    //Think now!
    public async Task PreloadAsset<T>(T objectType, IAssetPreload asset)
    {
        AssetReference assetReference = asset.AssetAddress;

        AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(assetReference);

        await handler.Task;

        T loadedAsset = handler.Result;

        Addressables.Release(handler);
    }

    public async void PreloadAssetWithAction<T>(T objectType, IAssetPreload asset, Action action)
    {
        await PreloadAsset(objectType, asset);

        action.Invoke();
    }

    public async void PreloadAssetWithAction<T, TAction>(T objectType, IAssetPreload asset, Action<TAction> action, TAction value)
    {
        await PreloadAsset(objectType, asset);

        action.Invoke(value);
    }
}