using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour, IObserver<EntityPoolManager>
{
    [SerializeField]
    private string iceTrailTag;

    private PlayerBoostAttackEvent PlayerBoostAttackEvent { get; set; }

    private EntityPoolManager EntityPoolManager { get; set; }

    private EntityPoolManagerDelegator EntityPoolManagerDelegator { get; set; }

    private List<GameObject> PooledIceTrails { get; set; } = new List<GameObject>();

    private async void Start()
    {
        EntityPoolManagerDelegator = await Helper.GetDelegator<EntityPoolManagerDelegator>();

        PlayerBoostAttackEvent = await Helper.GetCustomEvent<PlayerBoostAttackEvent>();

        EntityPoolManagerDelegator.NotifySubjectWrapper(this, Helper.BuildNotificationContext(gameObject.name, gameObject.tag, typeof(EntityPoolManager).ToString()), CancellationToken.None);
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

    private async Task<List<GameObject>> GetObjects(string tag)
    {
        List<GameObject> objects = new List<GameObject>();  

        List<EntityPool> entityPools = await GetPooledEntities(tag);

        if (entityPools.Count == 0)
        {
            throw new ApplicationException("Entity Pool Objects list is empty!");
        }

        entityPools.ForEach(entityPool => objects.Add((GameObject) entityPool.Entity));

        return objects;
    }

    private Task<List<EntityPool>> GetPooledEntities(string tag)
    {
        return Task.FromResult(EntityPoolManager.GetPooledEntity(tag));
    }

    public async void OnNotify(EntityPoolManager data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        EntityPoolManager = data;

        PooledIceTrails = await GetObjects(iceTrailTag);
    }
}