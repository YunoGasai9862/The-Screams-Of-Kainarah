using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour, INotify<EntityPoolManager>
{
    [SerializeField]
    private string iceTrailTag;

    private PlayerBoostAttackEvent PlayerBoostAttackEvent { get; set; }

    private EntityPoolManager EntityPoolManager { get; set; }

    private Delegator Delegator { get; set; }

    private List<GameObject> PooledIceTrails { get; set; } = new List<GameObject>();

    private async void Start()
    {
        Delegator = await Helper.GetDelegator<Delegator>();

        PlayerBoostAttackEvent = await Helper.GetCustomEvent<PlayerBoostAttackEvent>();

        Delegator.NotifySubjectWrapper(Helper.BuildNotificationContext<EntityPoolManager>(gameObject, typeof(EntityPoolManager)), this);
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