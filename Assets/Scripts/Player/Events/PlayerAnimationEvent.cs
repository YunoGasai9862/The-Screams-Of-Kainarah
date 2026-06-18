using System;
using System.Collections;
using System.Collections.Generic;
using Annotations.Enums;
using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using Assets.Scripts.Scene;
using UnityEngine;

[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(PlayerAnimationEvent), SubjectType = typeof(EntityPoolManager), ContextType = typeof(EntityPoolManager))]
public class PlayerAnimationEvent : MonoBehaviorScene, INotify<EntityPoolManager>
{
    [SerializeField]
    private string iceTrailTag;

    private PlayerBoostAttackEvent PlayerBoostAttackEvent { get; set; }

    private EntityPoolManager EntityPoolManager { get; set; }

    private Delegator Delegator { get; set; }

    private List<GameObject> PooledIceTrails { get; set; } = new List<GameObject>();

    private async void Start()
    {
       StartCoroutine(SceneUtils.GetDelegator<Delegator>(value => Delegator = value));

        PlayerBoostAttackEvent = await SceneUtils.GetCustomEvent<PlayerBoostAttackEvent>();

        Delegator.NotifySubjectWrapper(SceneUtils.BuildNotificationContext<EntityPoolManager>(gameObject, typeof(EntityPoolManager), typeof(PlayerAnimationEvent)), this);
    }

    public void IceTrailAnimation()
    {
        PooledIceTrails.ForEach(prefab =>
        {
            InstantiateUtility iceTrail = new(prefab);
            iceTrail.InstantiateObject(transform.position, Quaternion.identity);
            iceTrail.SetObjectsParent(transform);
        });
    }

    public void DesolveBoostAttack()
    {
        PlayerBoostAttackEvent.Invoke(false);
    }

    private List<GameObject> GetObjects(string tag)
    {
        List<GameObject> objects = new List<GameObject>();  

        List<EntityPool> entityPools = GetPooledEntities(tag);

        if (entityPools.Count == 0)
        {
            throw new ApplicationException("Entity Pool Objects list is empty!");
        }

        entityPools.ForEach(entityPool => objects.Add((GameObject) entityPool.Entity));

        return objects;
    }

    private List<EntityPool> GetPooledEntities(string tag)
    {
        return EntityPoolManager.GetPooledEntity(tag);
    }

    public IEnumerator Notify(EntityPoolManager value)
    {
        EntityPoolManager = value;

        PooledIceTrails = GetObjects(iceTrailTag);

        yield return null;
    }
}