using System;

public interface IAssetTypeParser
{
    public dynamic ParseType(object asset, AssetType assetType);

}