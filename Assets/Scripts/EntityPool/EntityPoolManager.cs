using Annotations.Enums;
using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using Assets.Scripts.BaseScene;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;

[Subject(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(EntityPoolManager), ContextType = typeof(EntityPoolManager))]
public class EntityPoolManager: MonoBehaviorScene, IDelegate, IEntityPoolManager, IRequest<EntityPoolManager>
{
    private Dictionary<string, List<EntityPool>> EntityPoolDict { get; set; } = new Dictionary<string, List<EntityPool>>();

    public IDelegate.InvokeMethod InvokeCustomMethod { get; set; }

    private SceneUtils SceneUtils { get; set; }

    private async Task Start()
    {
        InvokeCustomMethod += SetEntityPoolManagerDelegator;
    }

    public void Pool(EntityPool entityPool)
    {
        List<EntityPool> entities = EntityPoolDict.GetValueOrDefault(entityPool.Tag, new List<EntityPool>());

        entities.Add(entityPool);

        EntityPoolDict[entityPool.Tag] = entities;
    }
    public void UnPool(string tag)
    { 
        if (EntityPoolDict.TryGetValue(tag, out List<EntityPool> entities)) 
        {
            EntityPoolDict.Remove(tag);
        }
    }

    public void Activate(string tag)
    {
        if (EntityPoolDict.TryGetValue(tag, out List<EntityPool> entities))
        {
            foreach (EntityPool item in entities)
            {
                if (item.Entity is MonoBehaviour)
                {
                    GameObject EntityAsGameObject = (GameObject)item.Entity;

                    EntityAsGameObject.SetActive(true);
                }
            }
        }
    }
    public void Deactivate(string tag)
    {
        if (EntityPoolDict.TryGetValue(tag, out List<EntityPool> entities))
        {
            foreach (EntityPool item in entities)
            {
                if (item.Entity is MonoBehaviour)
                {
                    GameObject EntityAsGameObject = (GameObject)item.Entity;

                    EntityAsGameObject.SetActive(false);
                }
            }
        }
    }

    public List<EntityPool> GetPooledEntitiesWithAssetType(string tag, Asset assetType)
    {
        return EntityPoolDict.Where(kvp => kvp.Key == tag).SingleOrDefault().Value.Where(entityPool => entityPool.AssetType == assetType).ToList();
    }

    public List<EntityPool> GetPooledEntitiesWithAssetType(Asset assetType)
    {
        return EntityPoolDict.SelectMany(kvp => kvp.Value.Where(entityPool => entityPool.AssetType == assetType)).ToList();
    }

    public List<EntityPool> GetPooledEntity(string tag)
    {
        return EntityPoolDict.GetValueOrDefault(tag, new List<EntityPool>());
    }

    private async void SetEntityPoolManagerDelegator()
    {
        SceneUtils = await (await GetBaseScene()).GetSceneUtilsAsync();
    }

    public IEnumerator Request()
    {
       yield return StartCoroutine(SceneUtils.NotifyObservers(new SubjectContext<EntityPoolManager>() { Data = this, EntityType = typeof(EntityPoolManager)}, this));
    }
}