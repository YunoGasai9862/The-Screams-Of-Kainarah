using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class EntityPreloadScriptableObject : ScriptableObject, IEntityPreloadAction, IEntityType
{
    private EntityType m_entityType;
    public EntityType EntityIdentifier { get => m_entityType; set => m_entityType = EntityType.ScriptableObject; }
    public abstract Task EntityPreloadAction(AssetReference assetReference, EntityType entityType, Preloader preloader);
} 
