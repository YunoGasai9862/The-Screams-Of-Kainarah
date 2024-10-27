using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

public class EntityPoolManager: MonoBehaviour, IEntityPool
{
    [SerializeField]
    EntityPoolEvent entityPoolEvent;
    [SerializeField]
    EntityPoolManagerEvent entityPoolManagerEvent;
    [SerializeField]
    SceneSingletonActiveEvent sceneSingletonActiveEvent;

    private Dictionary<string, AbstractEntityPool> entityPoolDict = new Dictionary<string, AbstractEntityPool>();

    private void OnEnable()
    {
        entityPoolEvent.AddListener(InvokeEntityPool);

        sceneSingletonActiveEvent.AddListener(SceneSingletonActiveEventListener);
    }

    public Task Pool(AbstractEntityPool entityPool)
    {
        entityPoolDict.Add(entityPool.Tag, entityPool);

        Debug.Log(entityPool.ToString());

        return Task.CompletedTask;
    }
    public Task UnPool(string tag)
    {
        if (entityPoolDict.TryGetValue(tag, out AbstractEntityPool entityPool))
        {
            entityPoolDict.Remove(tag);
        }

        return Task.CompletedTask;
    }
    public async Task<AbstractEntityPool> GetPooledEntity(string tag)
    {
        TaskCompletionSource<AbstractEntityPool> tcs = new TaskCompletionSource<AbstractEntityPool>();

        if (entityPoolDict.TryGetValue(tag, out AbstractEntityPool entityPool))
        {
           bool resultSet = tcs.TrySetResult(entityPool);
        }

        return await tcs.Task;
    }

    public Task Activate(string tag)
    {
        TaskCompletionSource<AbstractEntityPool> tcs = new TaskCompletionSource<AbstractEntityPool>();

        if (entityPoolDict.TryGetValue(tag, out AbstractEntityPool entityPool))
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
        TaskCompletionSource<AbstractEntityPool> tcs = new TaskCompletionSource<AbstractEntityPool>();

        if (entityPoolDict.TryGetValue(tag, out AbstractEntityPool entityPool))
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

    private async void InvokeEntityPool(AbstractEntityPool entityPool)
    {
        await Pool(entityPool);
    }

    private void SceneSingletonActiveEventListener()
    {
        entityPoolManagerEvent.Invoke(this);
    }
}