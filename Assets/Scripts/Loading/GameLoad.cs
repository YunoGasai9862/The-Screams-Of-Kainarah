using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System;

public class GameLoad : MonoBehaviour, IGameLoad
{
    public async Task PreloadAsset<T>(IEntityPreload entityPreload)
    {
        AssetReference assetReference = entityPreload.AssetAddress;

        AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(assetReference);

        await handler.Task;

        T loadedAsset = handler.Result;

        Addressables.Release(handler);
    }

}