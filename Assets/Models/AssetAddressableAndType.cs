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
        [SerializeField]
        private string _assetType;
        public AssetReference AssetAddress { get => _assetAddress; set => _assetAddress = value; }
        public Type AssetType { get => GetAssetType(); }

        public Type GetAssetType()
        {
            Type type = Type.GetType(_assetType);
            return type;
        }
    }

    public AddressableAndType[] addressablesAndTypes;

}