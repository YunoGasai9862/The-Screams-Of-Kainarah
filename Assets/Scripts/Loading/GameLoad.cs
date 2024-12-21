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

    public async Task<UnityEngine.Object> PreloadAsset<T>(string label, Asset assetType)
    {
        AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(label);

        await handler.Task;

        T loadedAsset = handler.Result;

        UnityEngine.Object preloadedObject = await ProcessPreloadedAsset<T>(loadedAsset, assetType);

        Addressables.Release(handler);

        return preloadedObject;
    }

    public Task<UnityEngine.Object> ProcessPreloadedAsset<T>(T loadedAsset, Asset assetType)
    {
        switch (assetType)
        {
            case Asset.MONOBEHAVIOR:
                return Task.FromResult((UnityEngine.Object)Instantiate(loadedAsset as GameObject));

            case Asset.SCRIPTABLE_OBJECT:
                break;

            default:
                break;

        }

        return Task.FromResult(new UnityEngine.Object());
    }
} 