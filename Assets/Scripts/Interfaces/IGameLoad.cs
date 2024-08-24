using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
public interface IGameLoad
{
    public Task PreloadAsset<T>(AssetReference assetReference);
}