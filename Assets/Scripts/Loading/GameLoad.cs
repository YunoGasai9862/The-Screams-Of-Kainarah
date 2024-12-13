using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

public class GameLoad : MonoBehaviour, IGameLoad
{
    private GameLoadManager GameLoadManagerReference { get; set; }

    private async void Start()
    {
        GameLoadManagerReference = gameObject.GetComponentInParent<GameLoadManager>();

        await InvokeGameLoadManagerMethod();
    }

    private Task InvokeGameLoadManagerMethod()
    {

        GameLoadManagerReference.InvokeCustomMethod();

        return Task.CompletedTask;
    }

    //modify this to load on name/or type!
    public async Task<Object> PreloadAsset<T>(AssetReference assetReference, EntityType entityType)
    {
        AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(assetReference);

        await handler.Task;

        T loadedAsset = handler.Result;

        Object preloadedObject = await ProcessPreloadedAsset<T>(loadedAsset, entityType);

        Addressables.Release(handler);

        return preloadedObject;
    }

    public Task<Object> ProcessPreloadedAsset<T>(T loadedAsset, EntityType entityType)
    {
        if (HelperFunctions.IsEntityMonoBehavior(entityType))
        {
            return Task.FromResult((Object)Instantiate(loadedAsset as  GameObject));
        }

        return Task.FromResult(new Object());
    }
} 