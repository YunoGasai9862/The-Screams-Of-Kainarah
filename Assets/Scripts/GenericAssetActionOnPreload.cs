using System;

public class AssetActionOnPreload<T, TAction> : GenericAssetType<T>, IGenericAction<TAction>
{
    public Action<TAction> ExecuteActionOnLoad { get; private set; }

    public void SetAction(Action<TAction> action)
    {
        ExecuteActionOnLoad = action;
    }
}