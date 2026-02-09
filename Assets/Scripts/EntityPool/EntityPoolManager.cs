using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class EntityPoolManager: MonoBehaviour, IDelegate, IEntityPoolManager, ISubject<EntityPoolManager>
{
    private Dictionary<string, List<EntityPool>> EntityPoolDict { get; set; } = new Dictionary<string, List<EntityPool>>();

    public IDelegate.InvokeMethod InvokeCustomMethod { get; set; }

    private Delegator Delegator { get; set; }

    private void Start()
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

    public List<EntityPool> GetPooledEntity(string tag)
    {
        return EntityPoolDict.GetValueOrDefault(tag, new List<EntityPool>());
    }

    private async void SetEntityPoolManagerDelegator()
    {
        Delegator = await Helper.GetDelegator<Delegator>();
    }

    public void OnNotifySubject(IObserver<EntityPoolManager> data, ObserverContext context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(EntityPoolManagerDelegator.NotifyObserver(data, this, context, cancellationToken, semaphoreSlim));
    }
}