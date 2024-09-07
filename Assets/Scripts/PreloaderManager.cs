using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PreloaderManager: MonoBehaviour
{
    [SerializeField]
    Preloader preloader;

    [SerializeField]
    GameLoad gameLoad;

    [SerializeField]
    PreloadEntity[] preloadEntities;

    [SerializeField]
    ObjectPoolEvent objectPoolEvent;

    private async void Awake()
    {
        await InstantiateAndPoolGameLoad();
        await PreloadEntities(preloadEntities, preloader);
    }
    private async Task PreloadEntities(PreloadEntity[] preloadEntities, Preloader preloader)
    {
        foreach (var preloadEntity in preloadEntities)
        {
            Debug.Log($"Preload Entity: {preloadEntity.ToString()}");
            await preloadEntity.GetEntityToPreload().EntityPreloadAction(preloadEntity.AssetAddress, preloadEntity.PreloadEntityType, preloader);
        }
    }

    private async Task InstantiateAndPoolGameLoad()
    {
        GameLoad gameLoadObject = Instantiate(gameLoad);

        EntityPool entityPool = await EntityPool.From(gameLoadObject.name, gameLoadObject.tag, gameLoadObject.gameObject);

        await objectPoolEvent.Invoke(entityPool);

    }
}