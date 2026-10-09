using Annotations.Enums;
using Assets.Scripts.Loading.Models;
using Assets.Scripts.BaseScene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

public class PreloaderManager : MonoBehaviorScene
{
    [SerializeField]
    List<PreloadDto> poolObjects;

    [SerializeField]
    PreloadedEntitiesEvent preloadedEntitiesEvent;

    private List<UnityEngine.Object> PreloadedEntities { get; set; } = new List<UnityEngine.Object>();
    private EntityPoolManager EntityPoolManager { get; set; }
    private PositionalMarkerInstantiator MarkerManager { get; set; }

    private async void Start()
    {
        EntityPoolManager = FindFirstObjectByType<EntityPoolManager>();

        MarkerManager = FindFirstObjectByType<PositionalMarkerInstantiator>();

        Debug.Log($"EntityPoolManager: {EntityPoolManager}, MarkerManager: {MarkerManager}");

        List<PreloadDto> finalPoolEntities = new List<PreloadDto> { new PreloadDto(EntityPoolManager, Asset.MONOBEHAVIOR), new PreloadDto(MarkerManager, Asset.MONOBEHAVIOR) };

        await PoolEntites(finalPoolEntities, EntityPoolManager);

        await PreloadEntities(EntityPoolManager);
    }

    private async Task<AssetAttributeDto> GetAssetAttributesForPreloading()
    {
        List<AssetAttribute> assetAttributes = new List<AssetAttribute>();

        try
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            Debug.Log($"Found {types.Length} types in the assembly.");

            foreach (Type type in types)
            {
                AssetAttribute attribute = type.GetCustomAttribute<AssetAttribute>();

                Debug.Log($"Type: {type}, Attribute: {attribute}");

                if (attribute == null)
                {
                    Debug.Log($"Skipping type: {type}");
                    continue;
                }

                if (attribute.ExternalDependency)
                {
                    Debug.Log($"Skipping type: {type} because it's not an external dependency");
                    continue;
                }

                assetAttributes.Add(attribute);

            }
        }catch (Exception ex)
        {
            Debug.Log(ex.ToString());   
        }

        Debug.Log($"assetAttributes: {assetAttributes.Count}"); 

        List<AssetAttribute> untitledAssets = assetAttributes.Where(attribute => attribute.InstantiationOrder == 0).ToList();

        List<AssetAttribute> titledAssets = assetAttributes.Where(attribute => attribute.InstantiationOrder > 0).ToList();

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
            dynamic preloadedAsset = null;
            foreach(string markerId in asset.MarkerIds)
            {
                GameObject marker = MarkerManager.FindMarker(markerId);

                if (marker == null)
                {
                    Debug.Log($"Marker with ID {markerId} not found in the scene. Skipping asset initialization for this marker");
                    continue;
                }

                preloadedAsset = await MarkerManager.Load(asset.AssetType, asset.AddressLabel, marker);
            }


            if (preloadedAsset != null)
            {
                preloadedEntities.Add(await AddToPool(preloadedAsset, asset.AssetType, entityPoolManager));
            }
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

    private async Task PreloadEntities(EntityPoolManager entityPoolManager)
    {
        AssetAttributeDto assetAttributeDto =  await GetAssetAttributesForPreloading();
        
        Debug.Log($"Untitled Assets: {assetAttributeDto.UntitledAssets.Count}, Titled Assets: {assetAttributeDto.TitledAssets.Count}");

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

    private Task<T> InstantiateDependency<T>(GameObject dependency)
    {
        GameObject instantiatedDependency = Instantiate(dependency);

        PreloadedEntities.Add(instantiatedDependency);  

        return Task.FromResult(instantiatedDependency.GetComponent<T>());
    }
}