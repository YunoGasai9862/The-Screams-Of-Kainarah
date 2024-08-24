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
    private EntityPreload m_entityPreload;

    public AssetReference AssetAddress { get => m_assetReference;}

    public EntityPreload entityPreload { get => m_entityPreload;}
}