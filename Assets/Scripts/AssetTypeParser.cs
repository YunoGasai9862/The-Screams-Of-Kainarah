using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
public class AssetTypeParser: IAssetTypeParser
{
    public dynamic ParseType(object asset, AssetType assetType)
    {
        
        switch (assetType.GetType().Name)
        {
            case nameof(GameObject):
                return asset as GameObject;
            case nameof(ScriptableObject):
                return asset as ScriptableObject;
            default:
                break;
        }
     
        return null;
    }

}