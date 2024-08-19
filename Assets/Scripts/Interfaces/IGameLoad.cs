using System;
using System.Threading.Tasks;
public interface IGameLoad
{
    public Task PreloadAsset<T>(T objectType, IAssetPreload asset);
}