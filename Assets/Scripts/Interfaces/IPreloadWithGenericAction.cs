using System;
using System.Threading.Tasks;
public interface IPreloadWithGenericAction
{
    public Task PreloadAssetWithAction<T, TAction>(T objectType, IAssetPreload asset, Action<TAction> action, TAction value);

}