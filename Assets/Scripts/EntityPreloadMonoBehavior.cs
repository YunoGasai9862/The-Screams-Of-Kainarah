using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
public abstract class EntityPreloadMonoBehavior: MonoBehaviour, IEntityPreloadAction
{
    public abstract Task EntityPreloadAction(AssetReference assetReference, Preloader preloader);
}