using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
public interface IGameLoad
{
    public Task<UnityEngine.Object> PreloadAsset<T, Z>(Z label, Asset assetType) where T : UnityEngine.Object;
}