using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

public class ObjectPool: MonoBehaviour, IObjectPool
{

    [SerializeField]
    ObjectPoolEvent objectPoolEvent;

    private Queue<EntityPool> entityPoolQueue = new Queue<EntityPool>();

    private Dictionary<string, EntityPool> entityPoolDict = new Dictionary<string, EntityPool>();   

    private void Start()
    {
        objectPoolEvent.AddListener(InvokeEntityPool);
    }

    public Task Pool(EntityPool entityPool)
    {
        entityPoolQueue.Enqueue(entityPool);

        return Task.CompletedTask;
    }
    public Task UnPool(string tag)
    {
        EntityPool entityPool = new EntityPool();

        entityPoolDict.TryGetValue(tag, out entityPool);

        entityPoolQueue.Enqueue(entityPool);

        return Task.CompletedTask;
    }
    public Task<EntityPool> GetEntityPool(string tag)
    {
        EntityPool entityPool = new EntityPool();

        entityPoolDict.TryGetValue(tag, out entityPool);

        return Task.FromResult(entityPool);
    }

    public async void InvokeEntityPool(EntityPool entityPool)
    {
        entityPoolDict.Add(entityPool.Tag, entityPool);

        await Pool(entityPool);
    }
}