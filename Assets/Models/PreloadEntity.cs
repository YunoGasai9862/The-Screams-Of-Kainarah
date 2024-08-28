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
    private EntityPreloadMonoBehavior m_entityMB;
    [SerializeField]
    private EntityPreloadScriptableObject m_entitySO;
    [SerializeField]
    private bool m_isMonoBehavior;

    public AssetReference AssetAddress { get => m_assetReference;}

    public EntityPreloadMonoBehavior EntityMB { get => m_entityMB; }

    public EntityPreloadScriptableObject EntitySO { get => m_entitySO; }

    public bool IsMonoBehavior { get => m_isMonoBehavior;  }

}