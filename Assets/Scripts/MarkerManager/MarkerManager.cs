using Annotations.Enums;
using Assets.Scripts.BaseScene;
using Assets.Scripts.Interfaces;
using System;
using System.Threading.Tasks;
using UnityEngine;

[AssetAttribute(Asset.MONOBEHAVIOR, "MarkerManager", new string[0])]
public class MarkerManager : MonoBehaviorScene, IMarkerManager
{
    private GameLoad GameLoad { get; set; }

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

    public async Task<GameObject> Load(Asset assetType, GameObject marker)
    {
        switch(assetType)
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
                
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        GameLoad = FindFirstObjectByType<GameLoad>();
    }

}
