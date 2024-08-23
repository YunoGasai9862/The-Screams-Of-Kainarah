using System;
using System.Threading.Tasks;
public interface IPreloadWithGenericAction
{
    public Task PreloadAssetWithAction<T, TAction>(IEntityPreload asset, Action<TAction> action, TAction value);

}