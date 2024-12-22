using System;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

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
        PreloaderInstance = await InstantiatePreloader(preloader);

        await PreloadAssets();


        //await HelperFunctions.SetAsParent(PreloaderInstance.gameObject, gameObject);

        //await executePreloadingEvent.AddListener(ExecutePreloadingEventListener);
    }
    private async Task PreloadEntities(PreloadEntity[] preloadEntities, Preloader preloader)
    {
        foreach (PreloadEntity preloadEntity in preloadEntities)
        {
            //use reflection here!
          //  dynamic instance = await preloadEntity.GetEntityToPreload().EntityPreload(preloadEntity.AssetAddress, preloadEntity.PreloadEntityType, preloader);
          
          //  bool assetValueRefreshed = await RefreshInstance(instance, preloadEntity);

            await AddToPool(preloadEntity);

        }
    }

    private async Task PreloadAssets()
    {
        try
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (Type type in types)
            {
                AssetAttribute attribute = type.GetCustomAttribute<AssetAttribute>();
                //update this for both scriptable objects, etc + update method too to get something else instead of EntityType!!

                if (attribute == null)
                {
                    continue;
                }

                UnityEngine.Object preloadedAsset = await PreloadOnAssetType(attribute);
                Debug.Log(preloadedAsset);

            }
        }catch (Exception ex)
        {
            Debug.Log(ex.ToString());   
        }

    }

    private async Task Pool(string name, UnityEngine.Object entity, string tag)
    {
        EntityPool<UnityEngine.Object> entityPool = await EntityPool<UnityEngine.Object>.From(name, tag, entity);

        await entityPoolEvent.Invoke(entityPool);
    }

    private async Task AddToPool(PreloadEntity preloadEntity)
    {
       // if (HelperFunctions.IsEntityMonobehavior(preloadEntity.PreloadEntityType))
     //   {
         //   await Pool(preloadEntity.EntityMB.gameObject.name, preloadEntity.EntityMB.gameObject, preloadEntity.EntityMB.gameObject.tag);

     //   }else
      //  {
        //    await Pool(preloadEntity.EntitySO.name, preloadEntity.EntitySO, preloadEntity.EntitySO.name);
       // }
    }

    private async Task<bool> RefreshInstance(dynamic instance, PreloadEntity preloadEntity)
    {
        (EntityType entityType, dynamic dynamicInstance) = (Tuple<EntityType, dynamic>) instance;

        bool assetValueRefreshed = await RefreshOnType(dynamicInstance, entityType, preloadEntity);

        return assetValueRefreshed;
    }

    private async Task<UnityEngine.Object> PreloadOnAssetType(AssetAttribute attribute)
    {
        switch (attribute.AssetType)
        {
            case Asset.SCRIPTABLE_OBJECT:
                return await PreloaderInstance.PreloadAsset<ScriptableObject, string>(attribute.AddressLabel, attribute.AssetType);

            case Asset.MONOBEHAVIOR:
                return await PreloaderInstance.PreloadAsset<GameObject, string>(attribute.AddressLabel, attribute.AssetType);

            case Asset.NONE:
                break;

            default:
                break;
        }

        return new UnityEngine.Object();
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