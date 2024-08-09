using UnityEngine;
using System;
public class GenericAssetType<T>: AssetType
{
    public T Asset { get; set; }
    public override Type GetAssetType()
    {
        return typeof(T);
    }

}