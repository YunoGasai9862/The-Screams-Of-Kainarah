using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

public class GameLoad : MonoBehaviour, IGameLoad
{
    public async Task<Object> PreloadAsset<T>(AssetReference assetReference, EntityType entityType)
    {
        AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(assetReference);

        Debug.Log($"Handler: {handler}");

        await handler.Task;

        T loadedAsset = handler.Result;

        Debug.Log($"Loaded Asset: {loadedAsset}");

        Object preloadedObject = await ProcessPreloadedAsset<T>(loadedAsset, entityType);

        Addressables.Release(handler);

        return preloadedObject;
    }


    public Task<Object> ProcessPreloadedAsset<T>(T loadedAsset, EntityType entityType)
    {
        if (HelperFunctions.IsEntityMonoBehavior(entityType))
        {
            Debug.Log($"Instantiating MonoBehavior! {loadedAsset}");

            return Task.FromResult((Object)Instantiate(loadedAsset as  GameObject));
        }

        return Task.FromResult(new Object());
    }
} 