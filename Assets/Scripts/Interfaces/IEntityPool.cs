using System.Threading.Tasks;
public interface IEntityPool
{
    public Task Pool(AbstractEntityPool entityPool);
    public Task UnPool(string tag);
    public Task<AbstractEntityPool> GetEntityPool(string tag);
    public Task Activate(string tag);
    public Task Deactivate(string tag);
}