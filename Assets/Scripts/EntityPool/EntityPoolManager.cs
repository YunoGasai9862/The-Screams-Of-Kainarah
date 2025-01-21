using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class EntityPoolManager: MonoBehaviour, IEntityPoolManager, ISubjectAsync<IObserverAsync<EntityPoolManager>>
{
    [SerializeField]
    EntityPoolManagerDelegator entityPoolManagerDelegator;

    private Dictionary<string, EntityPool> entityPoolDict = new Dictionary<string, EntityPool>();

    private void Start()
    {
        entityPoolManagerDelegator.Subject.SetSubject(this);
    }

    public Task Pool(EntityPool entityPool)
    {
        entityPoolDict.Add(entityPool.Tag, entityPool);

        return Task.CompletedTask;
    }
    public Task UnPool(string tag)
    { 
        if (entityPoolDict.TryGetValue(tag, out EntityPool entityPool)) 
        {
            entityPoolDict.Remove(tag);
        }

        return Task.CompletedTask;
    }
    public async Task<EntityPool> GetPooledEntity(string tag)
    {
        TaskCompletionSource<EntityPool> tcs = new TaskCompletionSource<EntityPool>();

        if (entityPoolDict.TryGetValue(tag, out EntityPool entityPool))
        {
           bool resultSet = tcs.TrySetResult(entityPool);
        }

        return await tcs.Task;
    }

    public Task Activate(string tag)
    {
        TaskCompletionSource<EntityPool> tcs = new TaskCompletionSource<EntityPool>();

        if (entityPoolDict.TryGetValue(tag, out EntityPool entityPool))
        {
            if (entityPool.Entity is MonoBehaviour)
            {
                GameObject EntityAsGameObject = (GameObject)entityPool.Entity;
                EntityAsGameObject.SetActive(true);
                tcs.SetResult(entityPool);
            }
        }

        return tcs.Task;
    }
    public Task Deactivate(string tag)
    {
        TaskCompletionSource<EntityPool> tcs = new TaskCompletionSource<EntityPool>();

        if (entityPoolDict.TryGetValue(tag, out EntityPool entityPool))
        {
            if (entityPool.Entity is MonoBehaviour)
            {
                GameObject EntityAsGameObject = (GameObject)entityPool.Entity;
                EntityAsGameObject.SetActive(false);
                tcs.SetResult(entityPool);
            }
        }

        return tcs.Task;
    }

    public async Task OnNotifySubject(IObserverAsync<EntityPoolManager> data, params object[] optional)
    {
        await entityPoolManagerDelegator.NotifyObserver(data, this);
    }
}