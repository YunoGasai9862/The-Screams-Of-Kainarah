using System;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class PreloaderManager: Listener
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
    PreloadEntitiesEvent preloadEntitiesEvent;

    private Preloader PreloaderInstance { get; set; }

    private async void Start()
    {
        await PreloadAssets();

        //PreloaderInstance = await InstantiatePreloader(preloader);

        //await HelperFunctions.SetAsParent(PreloaderInstance.gameObject, gameObject);

        //await executePreloadingEvent.AddListener(ExecutePreloadingEventListener);
    }
    private async Task PreloadEntities(PreloadEntity[] preloadEntities, Preloader preloader)
    {
        foreach (PreloadEntity preloadEntity in preloadEntities)
        {
            //use reflection here!
            dynamic instance = await preloadEntity.GetEntityToPreload().EntityPreload(preloadEntity.AssetAddress, preloadEntity.PreloadEntityType, preloader);

            bool assetValueRefreshed = await RefreshInstance(instance, preloadEntity);

            await AddToPool(preloadEntity);

        }
    }

    private Task PreloadAssets()
    {
        List<AssetInfo> assets = new List<AssetInfo>();
        try
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (Type type in types)
            {
                AssetAttribute attribute = type.GetCustomAttribute<AssetAttribute>() ?? new AssetAttribute(Asset.NONE);

                switch(attribute.AssetType)
                {
                    case Asset.PRELOADING_SCRIPTABLE_OBJECT:
                            break;

                    case Asset.PRELOADING_MONOBEHAVIOR:
                            break;

                    case Asset.NONE:
                            break;

                    default:
                        break;
                }

                if (typeof(ScriptableObject).IsAssignableFrom(type))
                {
                    //okay this worked! continue on this :) u found the issue!
                    string assetKey = type.Name;
                    Debug.Log(assetKey);

                }
                else if (typeof(MonoBehaviour).IsAssignableFrom(type))
                {

                }
            }
        }catch (Exception ex)
        {
            Debug.Log(ex.ToString());   
        }

        return Task.CompletedTask;
    }

    private async Task Pool(string name, UnityEngine.Object entity, string tag)
    {
        EntityPool<UnityEngine.Object> entityPool = await EntityPool<UnityEngine.Object>.From(name, tag, entity);

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

    public override Task Listen()
    {
        preloadEntitiesEvent.Invoke(preloadEntities);

        return Task.CompletedTask;
    }
}