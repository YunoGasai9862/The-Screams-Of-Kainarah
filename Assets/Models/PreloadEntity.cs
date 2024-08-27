using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

[Serializable]
public class PreloadEntity
{
    [SerializeField]
    private AssetReference m_assetReference;
    [SerializeReference]
    private IEntityPreloadAction m_entity;

    public AssetReference AssetAddress { get => m_assetReference;}

    public IEntityPreloadAction Entity { get => m_entity; }
}