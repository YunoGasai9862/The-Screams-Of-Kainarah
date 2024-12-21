using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
public interface IEntityPreload
{
    public Task<Tuple<EntityType, dynamic>> EntityPreload(dynamic assetReference, Asset entityType, Preloader preloader);
}