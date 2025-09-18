using System;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PreloaderManager : MonoBehaviour
{
    [SerializeField]
    List<DependencyDto> dependencies;

    [SerializeField]
    PreloadedEntitiesEvent preloadedEntitiesEvent;

    private List<UnityEngine.Object> PreloadedEntities { get; set; } = new List<UnityEngine.Object>();

    private Preloader Preloader{ get; set; }
    private EntityPoolManager EntityPoolManager { get; set; }
    private GameLoad GameLoad { get; set; }

    private async void Start()
    {
        await InstantiateDependencies(dependencies);

        await PreloadEntities(Preloader, EntityPoolManager);
    }

    private async Task<List<AssetAttribute>> GetAssetAttributesForPreloading(Preloader preloader)
    {
        List<AssetAttribute> assetAttributes = new List<AssetAttribute>();

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

                Debug.Log($"AssetAttribute: {attribute.ToString()}");

                assetAttributes.Add(attribute);

            }
        }catch (Exception ex)
        {
            Debug.Log(ex.ToString());   
        }

        return assetAttributes.OrderBy(asset => asset.InstantiationOrder).ToList();
    }

    private async Task<List<UnityEngine.Object>> PreloadAssets(Preloader preloader, List<AssetAttribute> assets, EntityPoolManager entityPoolManager)
    {
        List<UnityEngine.Object> preloadedEntities = new List<UnityEngine.Object>();

        foreach (AssetAttribute asset in assets)
        {
            dynamic preloadedAsset = await PreloadOnAssetType(asset, preloader);

            preloadedEntities.Add(await AddToPool(preloadedAsset, entityPoolManager));
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
                return (ScriptableObject)await preloader.PreloadAsset<ScriptableObject>(
                    new PreloadPackage()
                    {
                        AddressableLable = attribute.AddressLabel,
                        AssetType = attribute.AssetType
                    }
                );
                    
            case Asset.MONOBEHAVIOR:
                return (GameObject)await preloader.PreloadAsset<GameObject>(new PreloadPackage()
                    {
                        AddressableLable = attribute.AddressLabel,
                        AssetType = attribute.AssetType,
                        InstantiateAt = new Vector3(attribute.InitialPositionX, attribute.InitialPositionY, attribute.InitialPositionZ)
                    }
                ); 

            case Asset.NONE:
                break;

            default:
                break;
        }

        return new UnityEngine.Object();
    }

    private async Task PreloadEntities(Preloader preloader, EntityPoolManager entityPoolManager)
    {
        List<AssetAttribute> assetsToPreload =  await GetAssetAttributesForPreloading(preloader);

        PreloadedEntities.AddRange(await PreloadAssets(preloader, assetsToPreload, entityPoolManager));

        await preloadedEntitiesEvent.Invoke(PreloadedEntities);
    }

    private async Task InstantiateDependencies(List<DependencyDto> dependencies)
    {
        foreach (DependencyDto dependency in dependencies)
        {
            switch(dependency.DependencyType)
            {
                case DependencyType.PRELOADER:
                    Preloader = await InstantiateDependency<Preloader>(dependency.Dependency);
                    break;
                case DependencyType.GAMELOAD:
                    GameLoad = await InstantiateDependency<GameLoad>(dependency.Dependency);
                    break;
                case DependencyType.ENTITYPOOL_MANAGER:
                    EntityPoolManager = await InstantiateDependency<EntityPoolManager>(dependency.Dependency);
                    break;
                default:
                    throw new ApplicationException($"Unknown dependency type found: {dependency.DependencyType}");
            }
        }
    }

    private Task<T> InstantiateDependency<T>(GameObject dependency)
    {
        GameObject instantiatedDependency = Instantiate(dependency);

        return Task.FromResult(instantiatedDependency.GetComponent<T>());
    }
}