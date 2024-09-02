using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
public interface IEntityPreloadAction
{
    public Task EntityPreloadAction(AssetReference assetReference, EntityType entityType, Preloader preloader);
}