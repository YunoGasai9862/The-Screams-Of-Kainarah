using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

public class EntityPoolManager: MonoBehaviour, IEntityPoolManager
{
    [SerializeField]
    EntityPoolManagerEvent entityPoolManagerEvent;
    [SerializeField]
    SceneSingletonActiveEvent sceneSingletonActiveEvent;

    private Dictionary<string, EntityPool> entityPoolDict = new Dictionary<string, EntityPool>();

    private void OnEnable()
    {
        sceneSingletonActiveEvent.AddListener(SceneSingletonActiveEventListener);
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

    private void SceneSingletonActiveEventListener()
    {
        entityPoolManagerEvent.Invoke(this);
    }
}