using UnityEngine;
using System;
public abstract class AssetType: MonoBehaviour, IAssetType, IAction
{
    public Action ExecuteActionOnLoad { get; }
    public abstract Type GetAssetType();
}

