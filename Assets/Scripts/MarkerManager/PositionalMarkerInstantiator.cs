using Annotations.Enums;
using Assets.Scripts.BaseScene;
using Assets.Scripts.Interfaces;
using System;
using System.Threading.Tasks;
using UnityEngine;

//for the ones marked as true, discard them from instantiation in the preloader!!
[AssetAttribute(Asset.MONOBEHAVIOR, "PositionalMarkerInstantiator", new string[0], true)]
public class PositionalMarkerInstantiator : MonoBehaviorScene, IMarkerManager
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

    public async Task<UnityEngine.Object> Load(Asset assetType, string addressLabel, GameObject marker)
    {
        switch(assetType)
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

    void Start()
    {
        GameLoad = FindFirstObjectByType<GameLoad>();

        Debug.Log($"GameLoad: {GameLoad} in MarkerManager");
    }

}
