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
    private EntityType m_entityType;

    public AssetReference AssetAddress { get => m_assetReference;}

    public EntityPreloadMonoBehavior EntityMB { get => m_entityMB; }

    public EntityPreloadScriptableObject EntitySO { get => m_entitySO; }

    public EntityType EntityType { get => m_entityType; }


    public IEntityPreloadAction GetEntityToPreload()
    {
        // return m_isMonoBehavior ? m_entityMB : m_entitySO;
        return null;
    }

}

//use Enum type!! This is better, why didn't you think this before?
public enum EntityType
{
    MonoBehavior,
    ScriptableObject
}