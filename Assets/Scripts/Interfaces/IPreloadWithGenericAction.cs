using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
public interface IPreloadWithGenericAction
{
    public Task PreloadAssetWithAction<T, TAction>(AssetReference assetReference, Action<TAction> action, TAction value);

}