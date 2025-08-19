using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameLoad : MonoBehaviour, IGameLoad
{
    //USE PRELOAD PACKAGE!!
    public async Task<UnityEngine.Object> PreloadAsset<T, Z>(Z label, Asset assetType, Vector3 instantiateAt) where T : UnityEngine.Object
    {
        AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(label);

        Debug.Log($"Handler: {handler}");

        await handler.Task;

        T loadedAsset = handler.Result;

        Debug.Log($"loadedAsset: {loadedAsset}");

        UnityEngine.Object preloadedObject = await ProcessPreloadedAsset<T>(loadedAsset, assetType, instantiateAt);

        Addressables.Release(handler);

        return preloadedObject;
    }

    public async Task<List<UnityEngine.Object>> PreloadAssets<Z>(Z label, Asset assetType, Vector3 instantiateAt)
    {
        List<UnityEngine.Object> assets = new List<UnityEngine.Object>();

        AsyncOperationHandle<IList<UnityEngine.Object>> handler = Addressables.LoadAssetsAsync<UnityEngine.Object>(label, null);

        await handler.Task;

        IList<UnityEngine.Object> loadedAsset = handler.Result.ToList();

        foreach(UnityEngine.Object asset in loadedAsset)
        {
           assets.Append(await ProcessPreloadedAsset<UnityEngine.Object>(asset, assetType, instantiateAt));
        }

        Addressables.Release(handler);

        return assets;
    }

    public Task<UnityEngine.Object> ProcessPreloadedAsset<T>(T loadedAsset, Asset assetType, Vector3 instantiateAt) where T : UnityEngine.Object
    {
        switch (assetType)
        {
            case Asset.MONOBEHAVIOR:
                return Task.FromResult((UnityEngine.Object) Instantiate(loadedAsset as GameObject, instantiateAt, Quaternion.identity));

            case Asset.SCRIPTABLE_OBJECT:
                return Task.FromResult((UnityEngine.Object)(loadedAsset as ScriptableObject));

            default:
                break;

        }

        return Task.FromResult(new UnityEngine.Object());
    }
} 