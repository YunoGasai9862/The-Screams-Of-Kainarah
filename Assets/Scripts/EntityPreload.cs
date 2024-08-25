using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
public abstract class EntityPreload: MonoBehaviour, IEntityPreload
{
    public abstract Task EntityPreloadAction(AssetReference assetReference, ActionPreloader actionPreloader);
}