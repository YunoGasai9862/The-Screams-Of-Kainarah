using UnityEngine;
using System;
public abstract class AssetType: MonoBehaviour, IAssetType
{
    public abstract Type GetAssetType();
}

