using System;
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

    private async Task RefreshInstance(dynamic instance, PreloadEntity preloadEntity)
    {
        (EntityType entityType, dynamic dynamicInstance) = (Tuple<EntityType, dynamic>) instance;

        await RefreshOnType(dynamicInstance, entityType, preloadEntity);
    }

    private Task RefreshOnType(dynamic instance, EntityType entityType, PreloadEntity preloadEntity)
    {
        switch (entityType)
        {
            case EntityType.MonoBehavior:
                GameObject castedObject = instance as GameObject;
                preloadEntity.EntityMB = castedObject.GetComponent<EntityPreloadMonoBehavior>();
                break;

            default:
                break;
        }
        return Task.CompletedTask;
    }
}