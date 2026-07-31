using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System.Linq;
using Annotations.Enums;
using Assets.Annotations;
using Assets.Scripts.BaseScene;

[Subject(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(GameLoad), ContextType = typeof(GameLoad))]
public class GameLoad : MonoBehaviorScene, IGameLoad
{
    public async Task<Object> PreloadAsset<T>(EntityMetaData enttityMetaData) where T : Object
    {
        AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(enttityMetaData.AddressableLabel);

        await handler.Task;

        T loadedAsset = handler.Result;

        Debug.Log($"loadedAsset: {loadedAsset}");

        Object preloadedObject = await ProcessPreloadedAsset<T>(loadedAsset, enttityMetaData);

        Addressables.Release(handler);

        return preloadedObject;
    }

    public async Task<List<Object>> PreloadAssets<Z>(Z label, EntityMetaData enttityMetaData)
    {
        List<Object> assets = new List<Object>();

        AsyncOperationHandle<IList<Object>> handler = Addressables.LoadAssetsAsync<Object>(label, null);

        await handler.Task;

        IList<Object> loadedAsset = handler.Result.ToList();

        foreach(Object asset in loadedAsset)
        {
           assets.Append(await ProcessPreloadedAsset(asset, enttityMetaData));
        }

        Addressables.Release(handler);

        return assets;
    }

    public Task<Object> ProcessPreloadedAsset<T>(T loadedAsset, EntityMetaData enttityMetaData) where T : Object
    {
        switch (enttityMetaData.AssetType)
        {
            case Asset.MONOBEHAVIOR:
                return Task.FromResult((Object) Instantiate(loadedAsset as GameObject, enttityMetaData.InstantiateAt, Quaternion.identity));

            case Asset.SCRIPTABLE_OBJECT:
                return Task.FromResult((Object)(loadedAsset as ScriptableObject));

            default:
                break;
        }

        return Task.FromResult(new Object());
    }
} 