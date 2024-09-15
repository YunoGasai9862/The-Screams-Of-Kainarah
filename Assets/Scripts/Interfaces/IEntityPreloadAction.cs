using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
public interface IEntityPreloadAction
{
    public Task<Tuple<EntityType, dynamic>> EntityPreloadAction(AssetReference assetReference, EntityType entityType, Preloader preloader);
}