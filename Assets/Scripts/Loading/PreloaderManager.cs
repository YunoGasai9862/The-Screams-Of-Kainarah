using System;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;

public class PreloaderManager : Listener
{
    [SerializeField]
    Preloader preloader;

    [SerializeField]
    EntityPoolManagerEvent entityPoolManagerEvent;

    [SerializeField]
    ExecutePreloadingEvent executePreloadingEvent;

    [SerializeField]
    PreloadedEntitiesEvent preloadedEntitiesEvent;

    private List<UnityEngine.Object> PreloadedEntities {get; set;}

    EntityPoolManager EntityPoolManager { get; set; }

    private Preloader PreloaderInstance { get; set; }

    private async void Start()
    {
        await entityPoolManagerEvent.AddListener(EntityPoolManagementEvent);

        PreloaderInstance = await InstantiatePreloader(preloader);

        await HelperFunctions.SetAsParent(PreloaderInstance.gameObject, gameObject);

        await executePreloadingEvent.AddListener(ExecutePreloading);
    }

    private async Task PreloadAssets(Preloader preloader)
    {
        try
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (Type type in types)
            {
                AssetAttribute attribute = type.GetCustomAttribute<AssetAttribute>();

                if (attribute == null)
                {
                    continue;
                }

                dynamic preloadedAsset = await PreloadOnAssetType(attribute, preloader);
                PreloadedEntities.Add(await AddToPool(preloadedAsset));
            }
        }catch (Exception ex)
        {
            Debug.Log(ex.ToString());   
        }

    }

    private async Task<UnityEngine.Object> AddToPool(dynamic entity)
    {
        if (entity is GameObject)
        {
           GameObject goEntity = (GameObject)entity;
           await EntityPoolManager.Pool(await EntityPool.From(goEntity.name, goEntity.tag, goEntity.gameObject));
           return goEntity;

        }else if (entity is ScriptableObject)
        {
            ScriptableObject soEntity = (ScriptableObject)entity;
            await EntityPoolManager.Pool(await EntityPool.From(soEntity.name, soEntity.name, soEntity));
            return soEntity;
        }

        return null;
    }

    private async Task<dynamic> PreloadOnAssetType(AssetAttribute attribute, Preloader preloader)
    {
        switch (attribute.AssetType)
        {
            case Asset.SCRIPTABLE_OBJECT:
                return (ScriptableObject) await preloader.PreloadAsset<ScriptableObject, string>(attribute.AddressLabel, attribute.AssetType);

            case Asset.MONOBEHAVIOR:
                return (GameObject) await preloader.PreloadAsset<GameObject, string>(attribute.AddressLabel, attribute.AssetType);

            case Asset.NONE:
                break;

            default:
                break;
        }

        return new UnityEngine.Object();
    }
    private async void ExecutePreloading()
    {
        await PreloadAssets(PreloaderInstance);

        await preloadedEntitiesEvent.Invoke(PreloadedEntities);
    }

    private Task<Preloader> InstantiatePreloader(Preloader preloader)
    {
        GameObject preloaderInstance = Instantiate(preloader.gameObject);

        return Task.FromResult(preloaderInstance.GetComponent<Preloader>());
    }

    private void EntityPoolManagementEvent(EntityPoolManager entityPoolManager)
    {
        EntityPoolManager = entityPoolManager;
    }

    public override Task Listen()
    {
        //do it some other way
        return Task.CompletedTask;
    }
}