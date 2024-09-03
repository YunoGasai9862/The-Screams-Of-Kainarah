using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

public class GameLoad : MonoBehaviour, IGameLoad
{
    public async Task PreloadAsset<T>(AssetReference assetReference, EntityType entityType)
    {
        AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(assetReference);

        await handler.Task;

        T loadedAsset = handler.Result;

        await ProcessPreloadedAsset<T>(loadedAsset, entityType);

        Addressables.Release(handler);
    }


    public Task ProcessPreloadedAsset<T>(T loadedAsset, EntityType entityType)
    {
        if (HelperFunctions.IsEntityMonoBehavior(entityType))
        {
            GameObject loadedAssetGO = Instantiate(loadedAsset as  GameObject);
        }

        return Task.CompletedTask;
    }
} 