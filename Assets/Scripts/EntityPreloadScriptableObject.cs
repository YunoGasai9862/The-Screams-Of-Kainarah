using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class EntityPreloadScriptableObject : ScriptableObject, IEntityPreloadAction
{
    public abstract Task EntityPreloadAction(AssetReference assetReference, Preloader preloader);
} 
