using System.Threading.Tasks;
public interface IObjectPool
{
    public Task Pool(EntityPool entityPool);
    public Task UnPool(string tag);
    public Task<EntityPool> GetEntityPool(string tag);
    public Task Activate(string tag);
    public Task Deactivate(string tag);
}