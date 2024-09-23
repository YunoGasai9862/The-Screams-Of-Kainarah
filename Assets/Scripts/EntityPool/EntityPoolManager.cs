using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

public class EntityPoolManager: MonoBehaviour, IEntityPool
{
    [SerializeField]
    EntityPoolEvent entityPoolEvent;
    [SerializeField]
    EntityPoolManagerActiveEvent entityPoolManagerActiveEvent;

    private Dictionary<string, dynamic> entityPoolDict = new Dictionary<string, dynamic>();

    private void OnEnable()
    {
        entityPoolManagerActiveEvent.Invoke(this);

        entityPoolEvent.AddListener(InvokeEntityPool);

    }

    public Task Pool<T>(EntityPool<T> entityPool)
    {
        entityPoolDict.Add(entityPool.Tag, entityPool);

        return Task.CompletedTask;
    }
    public Task UnPool<T>(string tag)
    {
        dynamic entityPool = new EntityPool<T>();

        if (entityPoolDict.TryGetValue(tag, out entityPool))
        {
            entityPoolDict.Remove(tag);
        }

        return Task.CompletedTask;
    }
    public async Task<EntityPool<T>> GetEntityPool<T>(string tag)
    {
        TaskCompletionSource<EntityPool<T>> tcs = new TaskCompletionSource<EntityPool<T>>();

        if (entityPoolDict.TryGetValue(tag, out var entityPool))
        {
           
           Debug.Log($"Entity Pool {entityPool} Tag: {tag} Type {entityPool.GetType()}");

           bool resultSet = tcs.TrySetResult(entityPool as EntityPool<T>);

           Debug.Log($"Task Await: {await tcs.Task as EntityPool<GameObject>} Result Set: {resultSet}");
        }


        return await tcs.Task as EntityPool<T>;
    }

    public Task Activate<T>(string tag)
    {
        dynamic entityPool = new EntityPool<T>();

        TaskCompletionSource<EntityPool<T>> tcs = new TaskCompletionSource<EntityPool<T>>();

        if (entityPoolDict.TryGetValue(tag, out entityPool))
        {
            entityPool.Entity.SetActive(true);

            tcs.SetResult(entityPool as EntityPool<T>);
        }

        return tcs.Task;
    }
    public Task Deactivate<T>(string tag)
    {
        dynamic entityPool = new EntityPool<T>();

        TaskCompletionSource<EntityPool<T>> tcs = new TaskCompletionSource<EntityPool<T>>();

        if (entityPoolDict.TryGetValue(tag, out entityPool))
        {
            entityPool.Entity.SetActive(false);

            tcs.SetResult(entityPool as EntityPool<T>);
        }

        return tcs.Task;
    }

    public async void InvokeEntityPool<T>(EntityPool<T> entityPool)
    {
        await Pool(entityPool);
    }
}