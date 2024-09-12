using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

public class ObjectPool: MonoBehaviour, IObjectPool
{
    [SerializeField]
    ObjectPoolEvent objectPoolEvent;
    [SerializeField]
    ObjectPoolActiveEvent objectPoolActiveEvent;

    private Dictionary<string, EntityPool> entityPoolDict = new Dictionary<string, EntityPool>();

    private void OnEnable()
    {
        objectPoolActiveEvent.Invoke(this);
        objectPoolEvent.AddListener(InvokeEntityPool);

    }

    public Task Pool(EntityPool entityPool)
    {
        Debug.Log($"Adding {entityPool.Tag} {entityPool}");
        entityPoolDict.Add(entityPool.Tag, entityPool);
        Debug.Log($"New Size {entityPoolDict.Count}");
        return Task.CompletedTask;
    }
    public Task UnPool(string tag)
    {
        EntityPool entityPool = new EntityPool();

        if (entityPoolDict.TryGetValue(tag, out entityPool))
        {
            entityPoolDict.Remove(tag);
        }

        return Task.CompletedTask;
    }
    public async Task<EntityPool> GetEntityPool(string tag)
    {
        EntityPool entityPool = new EntityPool();

        Debug.Log($"Size Of Pool {entityPoolDict.Count}");

        if (entityPoolDict.TryGetValue(tag, out entityPool))
        {
            return entityPool;
        }

        //foudn the error fix this tomorrow!!

        Debug.Log($"CHECKING ENTITY POOL : {entityPool.ToString()}");

        return null;
    }

    public Task Activate(string tag)
    {
        EntityPool entityPool = new EntityPool();

        TaskCompletionSource<EntityPool> tcs = new TaskCompletionSource<EntityPool>();

        if (entityPoolDict.TryGetValue(tag, out entityPool))
        {
            entityPool.Entity.SetActive(true);
            tcs.SetResult(entityPool);
        }

        return tcs.Task;
    }
    public Task Deactivate(string tag)
    {
        EntityPool entityPool = new EntityPool();

        TaskCompletionSource<EntityPool> tcs = new TaskCompletionSource<EntityPool>();

        if (entityPoolDict.TryGetValue(tag, out entityPool))
        {
            entityPool.Entity.SetActive(false);
            tcs.SetResult(entityPool);
        }

        return tcs.Task;
    }

    public async void InvokeEntityPool(EntityPool entityPool)
    {
        await Pool(entityPool);
    }
}