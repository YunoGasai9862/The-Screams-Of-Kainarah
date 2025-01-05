using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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

        Debug.Log("Here!!");

        GameLoadManagerReference.InvokeCustomMethod();

        return Task.CompletedTask;
    }

    public async Task<UnityEngine.Object> PreloadAsset<T, Z>(Z label, Asset assetType) where T : UnityEngine.Object
    {
        AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(label);

        await handler.Task;

        T loadedAsset = handler.Result;

        UnityEngine.Object preloadedObject = await ProcessPreloadedAsset<T>(loadedAsset, assetType);

        Addressables.Release(handler);

        return preloadedObject;
    }

    public async Task<List<UnityEngine.Object>> PreloadAssets<Z>(Z label, Asset assetType)
    {
        List<UnityEngine.Object> assets = new List<UnityEngine.Object>();

        AsyncOperationHandle<IList<UnityEngine.Object>> handler = Addressables.LoadAssetsAsync<UnityEngine.Object>(label, null);

        await handler.Task;

        IList<UnityEngine.Object> loadedAsset = handler.Result.ToList();

        foreach(UnityEngine.Object asset in loadedAsset)
        {
           assets.Append(await ProcessPreloadedAsset<UnityEngine.Object>(asset, assetType));
        }

        Addressables.Release(handler);

        return assets;
    }

    public Task<UnityEngine.Object> ProcessPreloadedAsset<T>(T loadedAsset, Asset assetType) where T : UnityEngine.Object
    {
        switch (assetType)
        {
            case Asset.MONOBEHAVIOR:
                return Task.FromResult((UnityEngine.Object) Instantiate(loadedAsset as GameObject));

            case Asset.SCRIPTABLE_OBJECT:
                return Task.FromResult((UnityEngine.Object)(loadedAsset as ScriptableObject));

            default:
                break;

        }

        return Task.FromResult(new UnityEngine.Object());
    }
} 