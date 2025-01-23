using System;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Collections;

public class PreloaderManager : MonoBehaviour, IObserver<EntityPoolManager>
{
    [SerializeField]
    Preloader preloader;

    [SerializeField]
    ExecutePreloadingEvent executePreloadingEvent;

    [SerializeField]
    PreloadedEntitiesEvent preloadedEntitiesEvent;

    [SerializeField]
    EntityPoolManagerDelegator entityPoolManagerDelegator;

    private List<UnityEngine.Object> PreloadedEntities {get; set;}

    private Preloader PreloaderInstance { get; set; }
    private EntityPoolManager EntityPoolManager { get; set; }

    private async void Start()
    {
        StartCoroutine(entityPoolManagerDelegator.NotifySubject(this));

        PreloadedEntities = new List<UnityEngine.Object>();

        PreloaderInstance = await InstantiatePreloader(preloader);

        await HelperFunctions.SetAsParent(PreloaderInstance.gameObject, gameObject);

        await executePreloadingEvent.AddListener(ExecutePreloading);
    }

    private async Task PreloadAssets(Preloader preloader, EntityPoolManager entityPoolManager)
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
                Debug.Log($"Preloaded Asset {preloadedAsset}");

                PreloadedEntities.Add(await AddToPool(preloadedAsset, entityPoolManager));
            }
        }catch (Exception ex)
        {
            Debug.Log(ex.ToString());   
        }

    }

    private async Task<UnityEngine.Object> AddToPool(dynamic entity, EntityPoolManager entityPoolManager)
    {
        if (entity is GameObject)
        {
           GameObject goEntity = (GameObject)entity;
           entityPoolManager.Pool(await EntityPool.From(goEntity.name, goEntity.tag, goEntity.gameObject));
           return goEntity;

        }else if (entity is ScriptableObject)
        {
            ScriptableObject soEntity = (ScriptableObject)entity;
            entityPoolManager.Pool(await EntityPool.From(soEntity.name, soEntity.name, soEntity));
            return soEntity;
        }

        return new UnityEngine.Object();
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
    private void ExecutePreloading()
    {
        StartCoroutine(ExecutePreloadAssets());
    }

    private IEnumerator ExecutePreloadAssets()
    {
        yield return new WaitUntil(() => EntityPoolManager != null);

        Debug.Log($"Logic Resumed: {EntityPoolManager}");

        _ = PreloadAssets(PreloaderInstance, EntityPoolManager);

        preloadedEntitiesEvent.Invoke(PreloadedEntities);
    }

    private Task<Preloader> InstantiatePreloader(Preloader preloader)
    {
        GameObject preloaderInstance = Instantiate(preloader.gameObject);

        return Task.FromResult(preloaderInstance.GetComponent<Preloader>());
    }

    public void OnNotify(EntityPoolManager data, params object[] optional)
    {
        EntityPoolManager = data;
    }
}