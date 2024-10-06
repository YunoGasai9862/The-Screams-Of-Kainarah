using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

public class GameLoad : MonoBehaviour, IGameLoad
{
    [SerializeField]
    GameLoadPoolEvent gameLoadPoolEvent;

    private void Awake()
    {
        Debug.Log("Invoking");

        gameLoadPoolEvent.Invoke();
    }

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