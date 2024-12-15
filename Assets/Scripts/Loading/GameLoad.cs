using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System;

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

    public async Task<UnityEngine.Object> PreloadAsset<T>(dynamic label, EntityType entityType)
    {
        AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(label);

        await handler.Task;

        T loadedAsset = handler.Result;

        UnityEngine.Object preloadedObject = await ProcessPreloadedAsset<T>(loadedAsset, entityType);

        Addressables.Release(handler);

        return preloadedObject;
    }

    private Task<Exception> CheckType(dynamic label)
    {
        if (label.GetType() != typeof(AssetReference) && label.GetType()!= typeof(string))
        {
            return Task.FromResult(new Exception("Label should be of AssetReference or string"));
        }

        return null;
    }
   

    public Task<UnityEngine.Object> ProcessPreloadedAsset<T>(T loadedAsset, EntityType entityType)
    {
        if (HelperFunctions.IsEntityMonoBehavior(entityType))
        {
            return Task.FromResult((UnityEngine.Object)Instantiate(loadedAsset as  GameObject));
        }

        return Task.FromResult(new UnityEngine.Object());
    }
} 