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

    public EntityPreloadMonoBehavior EntityMB { get => m_entityMB; set => m_entityMB = value; }

    public EntityPreloadScriptableObject EntitySO { get => m_entitySO; }

    public EntityType PreloadEntityType { get => m_entityType; }

    public override string ToString()
    {
        return $"Asset Reference: {AssetAddress}, EntityType: {PreloadEntityType}, EntityMB: {EntityMB}, EntitySO: {EntitySO}";
    }

    public IEntityPreload GetEntityToPreload()
    {
        switch (PreloadEntityType)
        {
            case EntityType.MonoBehavior:
                return EntityMB;

            case EntityType.ScriptableObject:
                return EntitySO;

            default:
                break;
        }
        return null;
    }

}
