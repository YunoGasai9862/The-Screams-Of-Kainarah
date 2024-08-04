using UnityEngine;
using System;
public class AssetType: MonoBehaviour, IAssetType<GameObject>
{
    public Type GetAssetType()
    {
        return typeof(GameObject);
    }
}