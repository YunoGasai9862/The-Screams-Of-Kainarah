using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

public class EntityPoolManager: MonoBehaviour, IEntityPool
{
    [SerializeField]
    EntityPoolEvent objectPoolEvent;
    [SerializeField]
    ObjectPoolActiveEvent objectPoolActiveEvent;

    private Dictionary<string, dynamic> entityPoolDict = new Dictionary<string, dynamic>();

    private void OnEnable()
    {
        objectPoolActiveEvent.Invoke(this);

        objectPoolEvent.AddListener(InvokeEntityPool);

    }

    public Task Pool<T>(EntityPool<T> entityPool)
    {
        entityPoolDict.Add(entityPool.Tag, entityPool);

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

        TaskCompletionSource<EntityPool> tcs = new TaskCompletionSource<EntityPool>();

        if (entityPoolDict.TryGetValue(tag, out entityPool))
        {
            tcs.SetResult(entityPool);
        }

        return await tcs.Task;
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

    public async void InvokeEntityPool<T>(EntityPool<T> entityPool)
    {
        await Pool(entityPool);
    }
}