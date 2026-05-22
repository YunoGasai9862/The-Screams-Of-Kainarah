using Annotations.Enums;
using System.Collections.Generic;
public interface IEntityPoolManager
{
    public void Pool(EntityPool entityPool);
    public void UnPool(string tag);
    public List<EntityPool> GetPooledEntity(string tag);
    public void Activate(string tag);
    public void Deactivate(string tag);
    public List<EntityPool> GetPooledEntitiesWithAssetType(string tag, Asset assetType);
    public List<EntityPool> GetPooledEntitiesWithAssetType(Asset assetType);
}