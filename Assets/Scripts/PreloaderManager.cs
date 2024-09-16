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

    [SerializeField]
    GameLoadPoolEvent gameLoadPoolEvent;

    private async void Awake()
    {
        await InstantiateAndPoolGameLoad();
        await PreloadEntities(preloadEntities, preloader);
    }
    private async Task PreloadEntities(PreloadEntity[] preloadEntities, Preloader preloader)
    {
        foreach (PreloadEntity preloadEntity in preloadEntities)
        {
             Debug.Log($"Preload Entity: {preloadEntity.ToString()}");

            //instantiate first

             dynamic instance = await preloadEntity.GetEntityToPreload().EntityPreloadAction(preloadEntity.AssetAddress, preloadEntity.PreloadEntityType, preloader);

             await RefreshInstance(instance, preloadEntity);

            //refresh the instance, and then use another interface to execute the action - separate preloading from action executing

        }
    }

    private async Task InstantiateAndPoolGameLoad()
    {
        GameLoad gameLoadObject = Instantiate(gameLoad);

        EntityPool entityPool =  await EntityPool.From(gameLoadObject.name, gameLoadObject.tag, gameLoadObject.gameObject);

        await objectPoolEvent.Invoke(entityPool);

        await gameLoadPoolEvent.Invoke(true);

    }

    private async Task<PreloadEntity> RefreshInstance(dynamic instance, PreloadEntity preloadEntity)
    {
        return await RefreshOnType(instance, preloadEntity);
    }

    private Task RefreshOnType(dynamic instance, PreloadEntity preloadEntity)
    {
        switch (preloadEntity.PreloadEntityType)
        {
            case EntityType.MonoBehavior:
                preloadEntity.EntityMB = (EntityPreloadMonoBehavior)instance;
                break;

            default:
                break;
        }
        return Task.CompletedTask;
    }
}