using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class EntityPoolManager: MonoBehaviour, IDelegate, IEntityPoolManager, ISubject<IObserver<EntityPoolManager>>
{
    private Dictionary<string, EntityPool> entityPoolDict = new Dictionary<string, EntityPool>();

    public IDelegate.InvokeMethod InvokeCustomMethod { get; set; }

    private EntityPoolManagerDelegator EntityPoolManagerDelegator { get; set; }

    private void Start()
    {
        InvokeCustomMethod += SetEntityPoolManagerDelegator;
    }

    public void Pool(EntityPool entityPool)
    {
        entityPoolDict.Add(entityPool.Tag, entityPool);
    }
    public void UnPool(string tag)
    { 
        if (entityPoolDict.TryGetValue(tag, out EntityPool entityPool)) 
        {
            entityPoolDict.Remove(tag);
        }
    }

    public void Activate(string tag)
    {
        if (entityPoolDict.TryGetValue(tag, out EntityPool entityPool))
        {
            if (entityPool.Entity is MonoBehaviour)
            {
                GameObject EntityAsGameObject = (GameObject)entityPool.Entity;

                EntityAsGameObject.SetActive(true);
            }
        }
    }
    public void Deactivate(string tag)
    {
        if (entityPoolDict.TryGetValue(tag, out EntityPool entityPool))
        {
            if (entityPool.Entity is MonoBehaviour)
            {
                GameObject EntityAsGameObject = (GameObject)entityPool.Entity;

                EntityAsGameObject.SetActive(false);
            }
        }
    }

    public EntityPool GetPooledEntity(string tag)
    {

        if (entityPoolDict.TryGetValue(tag, out EntityPool entityPool))
        {
            return entityPool;
        }

        return null;
    }

    private void SetEntityPoolManagerDelegator()
    {
        EntityPoolManagerDelegator.AddToSubjectsDict(typeof(EntityPoolManager).ToString(), name, new Subject<IObserver<EntityPoolManager>>());

        EntityPoolManagerDelegator.GetSubsetSubjectsDictionary(typeof(EntityPoolManager).ToString())[name].SetSubject(this);
    }

    public void OnNotifySubject(IObserver<EntityPoolManager> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(EntityPoolManagerDelegator.NotifyObserver(data, this, notificationContext, cancellationToken, semaphoreSlim));
    }
}