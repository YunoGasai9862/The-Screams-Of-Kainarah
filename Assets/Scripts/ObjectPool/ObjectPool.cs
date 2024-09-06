using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

public class ObjectPool: MonoBehaviour, IObjectPool
{
    private Queue<EntityPool> entityPoolQueue = new Queue<EntityPool>();
    public Task Pool(EntityPool entityPool)
    {
        return Task.CompletedTask;
    }
    public Task UnPool(string name)
    {
        return Task.CompletedTask;
    }
    public Task<EntityPool> GetEntityPool(string name)
    {
        return null;
    }

    public void InvokeEntityPool(EntityPool entityPool)
    {
        entityPoolQueue.Enqueue(entityPool);
    }
}