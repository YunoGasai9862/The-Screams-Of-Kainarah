using System.Threading.Tasks;
using UnityEngine;
public interface IGameLoadManager
{
    public abstract Task<GameObject> InstantiateAndPoolGameLoad(GameObject gameLoad, EntityPoolEvent entityPoolEvent);
}