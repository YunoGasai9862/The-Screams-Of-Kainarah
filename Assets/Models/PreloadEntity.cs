using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

[Serializable]
public class PreloadEntity
{
    [SerializeField]
    private AssetReference m_assetReference;
    [SerializeField]
    private EntityPreload m_entity;

    public AssetReference AssetAddress { get => m_assetReference;}

    public EntityPreload Entity { get => m_entity; }
}