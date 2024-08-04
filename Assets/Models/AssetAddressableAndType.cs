using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "AddressableAndTypes", menuName = "Addressables And Types")]
public class AssetAddressableAndType: ScriptableObject
{
    [Serializable]
    public class AddressableAndType
    {
        [SerializeField]
        private AssetReference _assetAddress;
        [SerializeReference]
        private IAssetTypeAbstract _assetType;
        public AssetReference AssetAddress { get => _assetAddress; set => _assetAddress = value; }
       // public IAssetTypeAbstract AssetType { get => _assetType; set => _assetType = value; }

    }

    public AddressableAndType[] addressablesAndTypes;

}