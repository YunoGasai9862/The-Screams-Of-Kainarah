using System.Threading.Tasks;
using UnityEngine;
public interface IEntityPoolManager
{
    public void Pool(EntityPool entityPool);
    public void UnPool(string tag);
    public EntityPool GetPooledEntity(string tag);
    public void Activate(string tag);
    public void Deactivate(string tag);
}