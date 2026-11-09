using Annotations.Enums;
using Assets.Annotations;
using Assets.Scripts.BaseScene;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

public class PositionalMarkerInstantiator : MonoBehaviorScene, IMarkerManager
{
    private GameLoad GameLoad { get; set; }

    public List<ChildAssetAttributeDto> FindChildAssets<T>(GameObject parent) where T : class
    {
        List<ChildAssetAttributeDto> childAssetDtos = new List<ChildAssetAttributeDto>();
        if (parent == null)
        {
            throw new ApplicationException("Parent object is null.");
        }

        GameObject[] children = parent.GetComponentsInChildren<Transform>(true).Select(T => T.gameObject).ToArray();

        children.ToList().ForEach(child =>
        {
            List<ChildAssetAttribute> assets = GetAssetAttributes<ChildAssetAttribute>(child.GetComponent<MonoBehaviour>());

            if (assets == null || assets.Count == 0)
            {
                throw new ApplicationException($"No ChildAssetAttribute found on child {child.name} of parent {parent.name}");
            }

            childAssetDtos.Add(new ChildAssetAttributeDto()
            {
                GameObject = child,
                ChildAssetAttributes = assets
            });

        });

        return childAssetDtos;
    }

    public List<T> GetAssetAttributes<T>(MonoBehaviour child) where T : Attribute
    {
        return child.GetType().GetCustomAttributes<T>().ToList();
    }

    public GameObject FindMarker(string markerName, string markerTag = "")
    {
        GameObject marker = GameObject.Find(markerName);

        if (marker == null)
        {
            Debug.Log($"Marker with name {markerName} not found.");
            return null;
        }

        return marker;
    }

    public async Task<UnityEngine.Object> Load(Asset assetType, bool enforceChildHierarchy, string addressLabel, GameObject marker)
    {
        if (GameLoad == null)
        {
            Debug.LogError("GameLoad is null in PositionalMarkerInstantiator. Cannot load MONOBEHAVIOR asset.");
            return null;
        }

        switch (assetType)
        {
            case Asset.SCRIPTABLE_OBJECT:
                return (ScriptableObject)await GameLoad.PreloadAsset<ScriptableObject>(
                    new EntityMetaData()
                    {
                        AddressableLabel = addressLabel,
                        AssetType = assetType
                    }
                );

            case Asset.MONOBEHAVIOR:
                Vector3 position = marker.gameObject.transform.position;
                if (enforceChildHierarchy)
                {
                    List<ChildAssetAttributeDto> childAssets = FindChildAssets<ChildAssetAttributeDto>(marker);
                    //you will need to initiate the child assets in the order specified and then link back to the main object
                }

                return (GameObject)await GameLoad.PreloadAsset<GameObject>(new EntityMetaData()
                {
                    AddressableLabel = addressLabel,
                    AssetType = assetType,
                    InstantiateAt = new Vector3(position.x, position.y, position.z)
                }
            );    
        }

        return null;
    }

    private void Awake()
    {
        GameLoad = FindFirstObjectByType<GameLoad>();

        Debug.Log($"GameLoad: {GameLoad} in MarkerManager");
    }
}
