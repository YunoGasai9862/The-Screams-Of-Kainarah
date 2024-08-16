using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System;

public class GameLoad : MonoBehaviour
{
    //polish this
    private async Task PreloadAsset<T, TAction>(T objectType, Action<TAction> action, TAction value, IAssetPreload asset)
    {
        AssetReference assetReference = asset.AssetAddress;

        AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(assetReference);

        await handler.Task;

        T loadedAsset = handler.Result;

        action.Invoke(value);

        Addressables.Release(handler);
    }
}