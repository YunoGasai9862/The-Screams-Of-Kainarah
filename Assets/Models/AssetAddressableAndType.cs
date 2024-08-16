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
        public AssetReference AssetAddress { get => _assetAddress; set => _assetAddress = value; }

    }

    public AddressableAndType[] addressablesAndTypes;

}