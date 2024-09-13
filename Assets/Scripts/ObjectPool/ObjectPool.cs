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

    public async void InvokeEntityPool(EntityPool entityPool)
    {
        await Pool(entityPool);
    }
}