using UnityEngine;
using System;
public class GenericAssetType<T>: AssetType
{
    public override Type GetAssetType()
    {
        return typeof(T);
    }

}