
public interface IAssetTypeParser<T>
{
    public T ParseType(object asset, AssetType assetType);

}