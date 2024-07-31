using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class AssetAddressablesAndType
{
    [SerializeField]
    private AssetReference _assetAddress;
    [SerializeField]
    private string _assetType;

    public AssetReference AssetAddress { get => _assetAddress; set => _assetAddress = value; }
    public string AssetType { get => _assetType; set => _assetType = value; }

    public void ParseType()
    {

    }
}