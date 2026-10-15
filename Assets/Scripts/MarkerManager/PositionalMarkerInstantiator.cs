using Annotations.Enums;
using Assets.Scripts.BaseScene;
using Assets.Scripts.Interfaces;
using System.Threading.Tasks;
using UnityEngine;

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
