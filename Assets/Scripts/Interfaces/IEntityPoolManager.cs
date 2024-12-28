using System.Threading.Tasks;
using UnityEngine;
public interface IEntityPoolManager
{
    public Task Pool(EntityPool entityPool);
    public Task UnPool(string tag);
    public Task<EntityPool> GetPooledEntity(string tag);
    public Task Activate(string tag);
    public Task Deactivate(string tag);
}