using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
public interface IGameLoad
{
    public Task<Object> PreloadAsset<T>(string label, Asset assetType);
}