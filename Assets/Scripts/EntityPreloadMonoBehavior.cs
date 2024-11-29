using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
public abstract class EntityPreloadMonoBehavior: MonoBehaviour, IEntityPreload, IEntityType
{
    private EntityType m_entityType;
    public EntityType EntityIdentifier { get => m_entityType; set => m_entityType = EntityType.MonoBehavior; }
    public virtual Task<Tuple<EntityType, dynamic>> EntityPreload(AssetReference assetReference, EntityType entityType, Preloader preloader)
    {
        return Task.FromResult(new Tuple<EntityType, dynamic>(EntityType.MonoBehavior, null)); //we return null as gameobject type, the child classes should implement that
    }
}