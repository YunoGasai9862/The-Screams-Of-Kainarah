using System;
using System.Threading.Tasks;
using UnityEngine;

public class PreloaderManager: MonoBehaviour
{
    [SerializeField]
    Preloader preloader;

    [SerializeField]
    PreloadEntity[] preloadEntities;

    [SerializeField]
    EntityPoolEvent entityPoolEvent;

    [SerializeField]
    ExecutePreloadingEvent executePreloadingEvent;

    [SerializeField]
    ExecuteDelegatesEvent executeDelegatesEvent;

    private Preloader PreloaderInstance { get; set; }

    private async void Start()
    {
        PreloaderInstance = await InstantiatePreloader(preloader);

        await HelperFunctions.SetAsParent(PreloaderInstance.gameObject, gameObject);

        await executePreloadingEvent.AddListener(ExecutePreloadingEventListener);
    }
    private async Task PreloadEntities(PreloadEntity[] preloadEntities, Preloader preloader)
    {
        foreach (PreloadEntity preloadEntity in preloadEntities)
        {
            dynamic instance = await preloadEntity.GetEntityToPreload().EntityPreload(preloadEntity.AssetAddress, preloadEntity.PreloadEntityType, preloader);

            bool assetValueRefreshed = await RefreshInstance(instance, preloadEntity);

            await AddToPool(preloadEntity);

        }
    }

    private async Task Pool<T>(string name, T entity, string tag)
    {
        EntityPool<T> entityPool = await EntityPool<T>.From(name, tag, entity);

        await entityPoolEvent.Invoke(entityPool);
    }

    private async Task AddToPool(PreloadEntity preloadEntity)
    {
        if (HelperFunctions.IsEntityMonoBehavior(preloadEntity.PreloadEntityType))
        {
            await Pool(preloadEntity.EntityMB.gameObject.name, preloadEntity.EntityMB.gameObject, preloadEntity.EntityMB.gameObject.tag);

        }else
        {
            await Pool(preloadEntity.EntitySO.name, preloadEntity.EntitySO, preloadEntity.EntitySO.name);
        }
    }

    private async Task<bool> RefreshInstance(dynamic instance, PreloadEntity preloadEntity)
    {
        (EntityType entityType, dynamic dynamicInstance) = (Tuple<EntityType, dynamic>) instance;

        bool assetValueRefreshed = await RefreshOnType(dynamicInstance, entityType, preloadEntity);

        return assetValueRefreshed;
    }

    private Task<bool> RefreshOnType(dynamic instance, EntityType entityType, PreloadEntity preloadEntity)
    {
        switch (entityType)
        {
            case EntityType.MonoBehavior:
                GameObject castedObject = instance as GameObject;
                preloadEntity.EntityMB = castedObject.GetComponent<EntityPreloadMonoBehavior>();
                return Task.FromResult(true);

            default:
                break;
        }
        return Task.FromResult(false);
    }

    private async void ExecutePreloadingEventListener()
    {
        await PreloadEntities(preloadEntities, PreloaderInstance);


    }

    private Task<Preloader> InstantiatePreloader(Preloader preloader)
    {
        GameObject preloaderInstance = Instantiate(preloader.gameObject);

        return Task.FromResult(preloaderInstance.GetComponent<Preloader>());
    }
}