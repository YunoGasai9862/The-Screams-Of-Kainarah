using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class EntityPoolManager: MonoBehaviour, IDelegate, IEntityPoolManager, ISubject<IObserver<EntityPoolManager>>
{
    private Dictionary<string, List<EntityPool>> entityPoolDict = new Dictionary<string, List<EntityPool>>();

    public IDelegate.InvokeMethod InvokeCustomMethod { get; set; }

    private EntityPoolManagerDelegator EntityPoolManagerDelegator { get; set; }

    private void Start()
    {
        InvokeCustomMethod += SetEntityPoolManagerDelegator;
    }

    public void Pool(EntityPool entityPool)
    {
        List<EntityPool> entities = entityPoolDict.GetValueOrDefault(entityPool.Tag, new List<EntityPool>());

        entities.Add(entityPool);

        entityPoolDict.Add(entityPool.Tag, entities);
    }
    public void UnPool(string tag)
    { 
        if (entityPoolDict.TryGetValue(tag, out List<EntityPool> entities)) 
        {
            entityPoolDict.Remove(tag);
        }
    }

    public void Activate(string tag)
    {
        if (entityPoolDict.TryGetValue(tag, out List<EntityPool> entities))
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
        if (entityPoolDict.TryGetValue(tag, out List<EntityPool> entities))
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

    public List<EntityPool> GetPooledEntity(string tag)
    {
        return entityPoolDict.GetValueOrDefault(tag, new List<EntityPool>());
    }

    private async void SetEntityPoolManagerDelegator()
    {
        EntityPoolManagerDelegator = await Helper.GetDelegator<EntityPoolManagerDelegator>();

        EntityPoolManagerDelegator.AddToSubjectsDict(typeof(EntityPoolManager).ToString(), name, new Subject<IObserver<EntityPoolManager>>());

        EntityPoolManagerDelegator.GetSubsetSubjectsDictionary(typeof(EntityPoolManager).ToString())[name].SetSubject(this);
    }

    public void OnNotifySubject(IObserver<EntityPoolManager> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(EntityPoolManagerDelegator.NotifyObserver(data, this, notificationContext, cancellationToken, semaphoreSlim));
    }
}