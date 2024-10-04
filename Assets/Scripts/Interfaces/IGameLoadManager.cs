using System.Threading.Tasks;
using UnityEngine;
public interface IGameLoadManager
{
    public abstract Task<GameLoad> InstantiateAndPoolGameLoad(GameLoad gameLoad, EntityPoolEvent entityPoolEvent);
}