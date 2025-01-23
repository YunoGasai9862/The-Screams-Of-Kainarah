using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class EntityPoolManager: MonoBehaviour, IEntityPoolManager, ISubject<IObserver<EntityPoolManager>>
{
    [SerializeField]
    EntityPoolManagerDelegator entityPoolManagerDelegator;

    private Dictionary<string, EntityPool> entityPoolDict = new Dictionary<string, EntityPool>();

    private void Start()
    {
        entityPoolManagerDelegator.Subject.SetSubject(this);
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

    public void OnNotifySubject(IObserver<EntityPoolManager> data, params object[] optional)
    {
       StartCoroutine(entityPoolManagerDelegator.NotifyObserver(data, this));
    }

    public EntityPool GetPooledEntity(string tag)
    {

        if (entityPoolDict.TryGetValue(tag, out EntityPool entityPool))
        {
            return entityPool;
        }

        return null;
    }
}