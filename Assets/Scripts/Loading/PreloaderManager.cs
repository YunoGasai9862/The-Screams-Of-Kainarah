using System;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Threading;

public class PreloaderManager : MonoBehaviour, IObserver<EntityPoolManager>
{
    [SerializeField]
    GameObject preloader;

    [SerializeField]
    ExecutePreloadingEvent executePreloadingEvent;

    [SerializeField]
    PreloadedEntitiesEvent preloadedEntitiesEvent;

    [SerializeField]
    public EntityPoolManagerDelegator entityPoolManagerDelegator;

    private List<UnityEngine.Object> PreloadedEntities { get; set; } = new List<UnityEngine.Object>();

    private Preloader PreloaderInstance { get; set; }
    private EntityPoolManager EntityPoolManager { get; set; }

    private async void Start()
    {
        StartCoroutine(entityPoolManagerDelegator.NotifySubject(this));

        PreloaderInstance = await InstantiatePreloader(preloader);

        await HelperFunctions.SetAsParent(PreloaderInstance.gameObject, gameObject);

        await executePreloadingEvent.AddListener(ExecutePreloading);
    }

    private async Task<List<UnityEngine.Object>> PreloadAssets(Preloader preloader, EntityPoolManager entityPoolManager)
    {
        List<UnityEngine.Object> preloadedEntities = new List<UnityEngine.Object>();

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

                preloadedEntities.Add(await AddToPool(preloadedAsset, entityPoolManager));
            }
        }catch (Exception ex)
        {
            Debug.Log(ex.ToString());   
        }

        return preloadedEntities;
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

    private async void SetPreloadedEntitiesAndInvokePreloadedEntitiesEvent(Preloader preloader, EntityPoolManager entityPoolManager)
    {
        PreloadedEntities.AddRange(await PreloadAssets(preloader, entityPoolManager));

        await preloadedEntitiesEvent.Invoke(PreloadedEntities);
    }

    private IEnumerator ExecutePreloadAssets()
    {
        yield return new WaitUntil(() => EntityPoolManager != null);

        SetPreloadedEntitiesAndInvokePreloadedEntitiesEvent(PreloaderInstance, EntityPoolManager);
    }

    private Task<Preloader> InstantiatePreloader(GameObject preloader)
    {
        GameObject preloaderInstance = Instantiate(preloader);

        return Task.FromResult(preloaderInstance.GetComponent<Preloader>());
    }

    public void OnNotify(EntityPoolManager data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        EntityPoolManager = data;
    }
}