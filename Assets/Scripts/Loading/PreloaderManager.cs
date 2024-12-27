using System;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;

public class PreloaderManager: Listener
{
    [SerializeField]
    Preloader preloader;

    [SerializeField]
    EntityPoolEvent entityPoolEvent;

    [SerializeField]
    ExecutePreloadingEvent executePreloadingEvent;

    private Preloader PreloaderInstance { get; set; }

    private async void Start()
    {
        PreloaderInstance = await InstantiatePreloader(preloader);

        await HelperFunctions.SetAsParent(PreloaderInstance.gameObject, gameObject);

        await executePreloadingEvent.AddListener(ExecutePreloadingEventListener);
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
                await AddToPool(preloadedAsset);
            }
        }catch (Exception ex)
        {
            Debug.Log(ex.ToString());   
        }

    }

    private async Task Pool<T>(string name, T entity, string tag)
    {
        EntityPool<T> entityPool = await EntityPool<T>.From(name, tag, entity);

        await entityPoolEvent.Invoke(entityPool);
    }

    private async Task AddToPool(dynamic entity)
    {
        if (entity is GameObject)
        {
           GameObject goEntity = (GameObject)entity;
           await Pool<GameObject>(goEntity.name, goEntity.gameObject, goEntity.tag);

        }else if (entity is ScriptableObject)
        {
            ScriptableObject soEntity = (ScriptableObject)entity;
            await Pool<ScriptableObject>(soEntity.name, soEntity, soEntity.name);
        }
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
    private async void ExecutePreloadingEventListener()
    {
        await PreloadAssets(PreloaderInstance);
    }

    private Task<Preloader> InstantiatePreloader(Preloader preloader)
    {
        GameObject preloaderInstance = Instantiate(preloader.gameObject);

        return Task.FromResult(preloaderInstance.GetComponent<Preloader>());
    }

    public override Task Listen()
    {
        //do it some other way
        return Task.CompletedTask;
    }
}