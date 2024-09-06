using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class EntityPool
{
    public string Name { get; set;}
    public string Tag { get; set; }
    public GameObject Entity { get ; set ; }
    public AssetReference AssetReference { get; set ; }
}