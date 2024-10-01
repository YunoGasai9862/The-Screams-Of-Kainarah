using System.Threading.Tasks;
public interface IGameLoadManager
{
    public abstract Task InstantiateAndPoolGameLoad(GameLoad gameLoad, EntityPoolEvent entityPoolEvent);
}