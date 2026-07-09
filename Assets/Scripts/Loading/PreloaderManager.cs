using System;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Annotations.Enums;
using Assets.Scripts.Scene;
using Assets.Scripts.Loading.Models;

public class PreloaderManager : Scene
{
    [SerializeField]
    List<PreloadDto> dependencies;

    [SerializeField]
    List<PreloadDto> poolObjects;

    [SerializeField]
    PreloadedEntitiesEvent preloadedEntitiesEvent;

    private List<UnityEngine.Object> PreloadedEntities { get; set; } = new List<UnityEngine.Object>();
    private EntityPoolManager EntityPoolManager { get; set; }
    private GameLoad GameLoad { get; set; }

    private async void Start()
    {
        await InstantiateDependencies(dependencies);

        await PoolEntites(poolObjects, EntityPoolManager);

        await PreloadEntities(EntityPoolManager);
    }

    private async Task<AssetAttributeDto> GetAssetAttributesForPreloading()
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

                assetAttributes.Add(attribute);

            }
        }catch (Exception ex)
        {
            Debug.Log(ex.ToString());   
        }

        List<AssetAttribute> untitledAssets = assetAttributes.Where(attribute => attribute.InstantiationOrder == 0).ToList();

        List<AssetAttribute> titledAssets = assetAttributes.Where(attribute => attribute.InstantiationOrder > 0).ToList();
        //do the instantiation for those here at last!

        if (titledAssets.GroupBy(asset => asset.InstantiationOrder).Any(group => group.Count() > 1))
        {
            throw new ApplicationException($"Multiple assets found with the same instantiation order. Please ensure all assets have a unique instantiation order.");
        }

        return new AssetAttributeDto
        {
            TitledAssets = titledAssets.OrderBy(asset => asset.InstantiationOrder).ToList(),
            UntitledAssets = untitledAssets
        };
    }

    private async Task<List<UnityEngine.Object>> PreloadAssets(List<AssetAttribute> assets, EntityPoolManager entityPoolManager)
    {
        List<UnityEngine.Object> preloadedEntities = new List<UnityEngine.Object>();

        foreach (AssetAttribute asset in assets)
        {
            Debug.Log($"Asset: {asset}");
            dynamic preloadedAsset = await PreloadOnAssetType(asset);

            preloadedEntities.Add(await AddToPool(preloadedAsset, asset.AssetType, entityPoolManager));
        }

        return preloadedEntities;
    }


    private async Task<UnityEngine.Object> AddToPool(dynamic entity, Asset assetType, EntityPoolManager entityPoolManager)
    {
        switch(assetType)
        {
            case Asset.SCRIPTABLE_OBJECT:
                ScriptableObject soEntity = (ScriptableObject)entity;
                entityPoolManager.Pool(await EntityPool.From(soEntity.name, soEntity.name, assetType, soEntity));
                return soEntity;

            case Asset.MONOBEHAVIOR:
                GameObject goEntity = (GameObject)entity;
                entityPoolManager.Pool(await EntityPool.From(goEntity.name, goEntity.tag, assetType, goEntity.gameObject));
                return goEntity;
        }

        return new UnityEngine.Object();
    }

    private async Task<dynamic> PreloadOnAssetType(AssetAttribute attribute)
    {
        switch (attribute.AssetType)
        {
            case Asset.SCRIPTABLE_OBJECT:
                return (ScriptableObject)await GameLoad.PreloadAsset<ScriptableObject>(
                    new EntityMetaData()
                    {
                        AddressableLabel = attribute.AddressLabel,
                        AssetType = attribute.AssetType
                    }
                );
                    
            case Asset.MONOBEHAVIOR:
                return (GameObject)await GameLoad.PreloadAsset<GameObject>(new EntityMetaData()
                    {
                        AddressableLabel = attribute.AddressLabel,
                        AssetType = attribute.AssetType,
                        InstantiateAt = new Vector3(attribute?.InitialPositionX ?? 0.0f, attribute?.InitialPositionY ?? 0.0f, attribute?.InitialPositionZ ?? 0.0f)
                    }
                ); 

            default:
                break;
        }

        return new UnityEngine.Object();
    }

    private async Task PreloadEntities(EntityPoolManager entityPoolManager)
    {
        AssetAttributeDto assetAttributeDto =  await GetAssetAttributesForPreloading();

        PreloadedEntities.AddRange(await PreloadAssets(assetAttributeDto.UntitledAssets, entityPoolManager));

        PreloadedEntities.AddRange(await PreloadAssets(assetAttributeDto.TitledAssets, entityPoolManager));

        await preloadedEntitiesEvent.Invoke(PreloadedEntities);
    }

    private async Task PoolEntites(List<PreloadDto> entities, EntityPoolManager entityPoolManager)
    {
        foreach (PreloadDto item in poolObjects)
        {
            await AddToPool(item.Entity, item.AssetType, entityPoolManager);
        }
    }

    private async Task InstantiateDependencies(List<PreloadDto> dependencies)
    {
        foreach (PreloadDto dependency in dependencies)
        {
            switch(dependency.PreloadEntityType)
            {
                case PreloadEntityType.GAMELOAD:
                    GameLoad = await InstantiateDependency<GameLoad>(dependency.Entity);
                    break;
                case PreloadEntityType.ENTITYPOOL_MANAGER:
                    EntityPoolManager = await InstantiateDependency<EntityPoolManager>(dependency.Entity);
                    break;
                default:
                    throw new ApplicationException($"Unknown dependency type found: {dependency.PreloadEntityType}");
            }
        }
    }

    private Task<T> InstantiateDependency<T>(GameObject dependency)
    {
        GameObject instantiatedDependency = Instantiate(dependency);

        PreloadedEntities.Add(instantiatedDependency);  

        return Task.FromResult(instantiatedDependency.GetComponent<T>());
    }
}