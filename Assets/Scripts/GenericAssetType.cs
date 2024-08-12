using UnityEngine;
using System;
public abstract class GenericAssetType<T, TAction> : AssetType, IGenericAction<TAction>
{
    public T Asset { get; set; }

    public new Action<TAction> ExecuteActionOnLoad { get; private set; }

    public override Type GetAssetType()
    {
        return typeof(T);
    }
    public Type GetActionType()
    {
        return typeof(TAction);
    }

    public void SetAction(Action<TAction> action)
    {
        ExecuteActionOnLoad = action;
    }

}