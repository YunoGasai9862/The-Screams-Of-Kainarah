using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
public interface IEntityPreload
{
    public Task<Tuple<EntityType, dynamic>> EntityPreload(AssetReference assetReference, EntityType entityType, Preloader preloader);
}