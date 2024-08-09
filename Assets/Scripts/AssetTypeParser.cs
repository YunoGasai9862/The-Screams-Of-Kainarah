using System;
using UnityEngine;
public class AssetTypeParser: IAssetTypeParser<AssetType>
{
    public AssetType ParseType(object asset, AssetType assetType)
    {
        if(assetType.GetType() == typeof(GameObject))
        {
            return new GenericAssetType<GameObject> { Asset = asset as GameObject };

        }else if(assetType.GetType() == typeof(ScriptableObject)){

            return new GenericAssetType<ScriptableObject> { Asset = asset as ScriptableObject };    
        }
        
        return null;
    }

}