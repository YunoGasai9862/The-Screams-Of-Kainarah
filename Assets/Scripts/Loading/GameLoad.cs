using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System;

public class GameLoad : MonoBehaviour, IGameLoad
{
    [SerializeField]
    IEntityPreload[] entitiesPreload;

    //refactor more to grab IAssetPreload directly from IEntityPreload - group them!
    public async Task PreloadAsset<T>(T objectType, IAssetPreload asset)
    {
        foreach(IEntityPreload entityPreload in entitiesPreload)
        {
            AssetReference assetReference = asset.AssetAddress;

            AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(assetReference);

            await handler.Task;

            T loadedAsset = handler.Result;

            Addressables.Release(handler);
        }

    }
}