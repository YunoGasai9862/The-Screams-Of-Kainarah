using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class AssetAddressableAndType
{
    [SerializeField]
    private AssetReference _assetAddress;
    [SerializeField]
    private string _assetType;

    public AssetReference AssetAddress { get => _assetAddress; set => _assetAddress = value; }
    public Type AssetType { get => GetAssetType();}

    public Type GetAssetType()
    {
        Type type = Type.GetType(_assetType);
        return type;
    }
}