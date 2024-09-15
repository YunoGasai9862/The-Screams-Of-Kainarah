using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class EntityPreloadScriptableObject : ScriptableObject, IEntityPreload, IEntityPreloadAction, IEntityType
{
    private EntityType m_entityType;
    public EntityType EntityIdentifier { get => m_entityType; set => m_entityType = EntityType.ScriptableObject; }
    public abstract Task<Tuple<EntityType, dynamic>> EntityPreloadAction(AssetReference assetReference, EntityType entityType, Preloader preloader);
} 
