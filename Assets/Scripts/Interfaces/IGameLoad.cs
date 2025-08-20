using System;
using System.Threading.Tasks;

public interface IGameLoad
{
    public Task<UnityEngine.Object> PreloadAsset<T>(PreloadPackage preloadPackage) where T : UnityEngine.Object;
}