using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class EntityPreloadScriptableObject : ScriptableObject, IEntityPreload, IEntityType
{
    private EntityType m_entityType;
    public EntityType EntityIdentifier { get => m_entityType; set => m_entityType = EntityType.ScriptableObject; }
    public virtual Task<Tuple<EntityType, dynamic>> EntityPreload(AssetReference assetReference, EntityType entityType, Preloader preloader)
    {
        return Task.FromResult(new Tuple<EntityType, dynamic>(EntityType.ScriptableObject, null)); //we return null as gameobject type, the child classes should implement that
    }
} 
