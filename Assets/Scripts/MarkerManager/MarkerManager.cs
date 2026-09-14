using Assets.Scripts.BaseScene;
using Assets.Scripts.Interfaces;
using UnityEngine;

[AssetAttribute(Annotations.Enums.Asset.MONOBEHAVIOR, "MarkerManager", new string[0])]
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        
    }

}
