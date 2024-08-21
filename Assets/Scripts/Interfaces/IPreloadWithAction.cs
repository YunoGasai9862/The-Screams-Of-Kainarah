
using System.Threading.Tasks;
using System;
public interface IPreloadWithAction
{
    public Task PreloadAssetWithAction<T>(T objectType, IAssetPreload asset, Action action);
}