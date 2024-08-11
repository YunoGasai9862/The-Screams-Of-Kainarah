using System;
public abstract class AssetActionOnPreload: AssetType, IAction
{
    public Action ExecuteActionOnLoad { get; }
}