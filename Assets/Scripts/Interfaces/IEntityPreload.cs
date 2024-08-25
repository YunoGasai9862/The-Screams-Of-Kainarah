using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
public interface IEntityPreload
{
    public Task EntityPreloadAction(AssetReference assetReference, ActionPreloader actionPreloader);
}