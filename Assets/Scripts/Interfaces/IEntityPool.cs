using System.Threading.Tasks;
public interface IEntityPool
{
    public Task Pool<T>(EntityPool<T> entityPool);
    public Task UnPool(string tag);
    public Task<EntityPool<T>> GetEntityPool<T>(string tag);
    public Task Activate(string tag);
    public Task Deactivate(string tag);
}