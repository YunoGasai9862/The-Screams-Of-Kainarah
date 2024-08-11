using System;

public class AssetActionOnPreload<T, Action>: GenericAssetType<T>, IAction<Action>
{
    public Action<Action> ExecuteActionOnLoad { get; set; }
}