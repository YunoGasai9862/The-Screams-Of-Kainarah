
using System.Threading.Tasks;
using System;
public interface IPreloadWithAction
{
    public Task PreloadAssetWithAction<T>(IEntityPreload asset, Action action);
}