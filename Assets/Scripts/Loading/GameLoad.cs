using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameLoad : MonoBehaviour, IGameLoad
{
    public async Task<UnityEngine.Object> PreloadAsset<T>(PreloadPackage preloadPackage) where T : UnityEngine.Object
    {
        AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(preloadPackage.AddressableLable);

        Debug.Log($"Handler: {handler}");

        await handler.Task;

        T loadedAsset = handler.Result;

        Debug.Log($"loadedAsset: {loadedAsset}");

        UnityEngine.Object preloadedObject = await ProcessPreloadedAsset<T>(loadedAsset, preloadPackage);

        Addressables.Release(handler);

        return preloadedObject;
    }

    public async Task<List<UnityEngine.Object>> PreloadAssets<Z>(Z label, PreloadPackage preloadPackage)
    {
        List<UnityEngine.Object> assets = new List<UnityEngine.Object>();

        AsyncOperationHandle<IList<UnityEngine.Object>> handler = Addressables.LoadAssetsAsync<UnityEngine.Object>(label, null);

        await handler.Task;

        IList<UnityEngine.Object> loadedAsset = handler.Result.ToList();

        foreach(UnityEngine.Object asset in loadedAsset)
        {
           assets.Append(await ProcessPreloadedAsset(asset, preloadPackage));
        }

        Addressables.Release(handler);

        return assets;
    }

    public Task<UnityEngine.Object> ProcessPreloadedAsset<T>(T loadedAsset, PreloadPackage preloadPackage) where T : UnityEngine.Object
    {
        switch (preloadPackage.AssetType)
        {
            case Asset.MONOBEHAVIOR:
                return Task.FromResult((UnityEngine.Object) Instantiate(loadedAsset as GameObject, preloadPackage.InstantiateAt, Quaternion.identity));

            case Asset.SCRIPTABLE_OBJECT:
                return Task.FromResult((UnityEngine.Object)(loadedAsset as ScriptableObject));

            default:
                break;

        }

        return Task.FromResult(new UnityEngine.Object());
    }
} 