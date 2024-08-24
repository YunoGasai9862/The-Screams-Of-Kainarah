
using System.Threading.Tasks;
using System;
using UnityEngine.AddressableAssets;
public interface IPreloadWithAction
{
    public Task PreloadAssetWithAction<T>(AssetReference assetReference, Action action);
}