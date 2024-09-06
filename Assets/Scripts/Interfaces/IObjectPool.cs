using System.Threading.Tasks;
public interface IObjectPool
{
    public Task Pool(EntityPool entityPool);
    public Task UnPool(string name);
    public Task<EntityPool> GetEntityPool(string name);
}